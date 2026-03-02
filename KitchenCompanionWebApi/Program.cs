
using KitchenCompanionWebApi.Models.DatabaseFirst;
using KitchenCompanionWebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System;
using System.Text;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel();

// Add services to the container.
/* 
 * https://localhost:7222/scalar/v1
 * http://0.0.0.0:5285/scalar/v1
 * */

builder.Services.AddControllers();

//builder.Services.AddDbContext<RecipeEntitiesContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("UserDatabase")));

builder.Services.AddDbContext<RecipeEntitiesContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("UserDatabase");

    Console.WriteLine("========== ACTUAL CONNECTION STRING ==========");
    Console.WriteLine(cs ?? "<NULL>");
    Console.WriteLine("==============================================");

    options.UseSqlServer(cs);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("https://recipetracker.xyz")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
var key = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!)
);

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

var tokenParams = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidIssuer = builder.Configuration["AppSettings:Issuer"],

    ValidateAudience = true,
    ValidAudience = builder.Configuration["AppSettings:Audience"],

    ValidateLifetime = true,

    ValidateIssuerSigningKey = true,
    IssuerSigningKey = key
};

builder.Services.AddAuthentication()
    // ===== NORMAL BEARER (Authorization header) =====
    .AddJwtBearer("JwtBearer", options =>
    {
        options.TokenValidationParameters = tokenParams;
    })

    // ===== COOKIE JWT =====
    .AddJwtBearer("JwtCookie", options =>
    {
        options.TokenValidationParameters = tokenParams;

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["access_token"];
                return Task.CompletedTask;
            }
        };
    });
/*
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["AppSettings:Issuer"], 
        ValidateAudience = true, 
        ValidAudience = builder.Configuration["AppSettings:Audience"], 
        ValidateLifetime = true, 
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!)),
        ValidateIssuerSigningKey = true
    };

    // remove me
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["access_token"];
            return Task.CompletedTask;
        }
    };
}); **/

builder.Services.AddScoped<IAuthService, AuthService>(); 
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
//app.Urls.Add("http://0.0.0.0:5285");
// MUST BE BEFORE MapControllers
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                       ForwardedHeaders.XForwardedProto
});
app.UseRouting(); 

app.UseCors(x => x
    .WithOrigins("https://recipetracker.xyz")   // frontend
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
);
/*app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);**/


//app.UseCors("AllowFrontend");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
// https://chatgpt.com/share/6947490f-1280-8009-b2ad-ec0f3c6f455e
app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        @"/home/uploadedimages"),
    RequestPath = "/uploads"
});

app.UseAuthentication(); // REMOVE ME
app.UseAuthorization();

app.MapControllers();

app.Run();
