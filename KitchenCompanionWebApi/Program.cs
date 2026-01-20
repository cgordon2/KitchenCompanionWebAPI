
using KitchenCompanionWebApi.Models.DatabaseFirst;
using KitchenCompanionWebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System;
using System.Text;

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
}); 

builder.Services.AddScoped<IAuthService, AuthService>(); 
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
//app.Urls.Add("http://0.0.0.0:5285");



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
        @"C:\Users\gordo\OneDrive\Desktop\GH_Final_WebAPI\KitchenCompanionWebAPI\KitchenCompanionWebApi\UploadedImages"),
    RequestPath = "/uploads"
});
app.UseAuthorization();

app.MapControllers();

app.Run();
