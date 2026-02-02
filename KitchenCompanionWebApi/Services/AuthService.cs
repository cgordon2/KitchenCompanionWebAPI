using KitchenCompanionWebApi.Models.DatabaseFirst; 
using KitchenCompanionWebApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KitchenCompanionWebApi.Models.DTOs;

namespace KitchenCompanionWebApi.Services
{
    public class AuthService(RecipeEntitiesContext context, IConfiguration configuration ) : IAuthService
    {
        public async Task<string?> LoginAsync(UserDto request)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == request.Username); 

            if (user is null)
            {
                return null; 
            }

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.UserPassword, request.Password)
                == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return CreateToken(user); 
        }

        public async Task<List<UserFollowerDto>> GetFollowers(int currentUserId)
        {
            return await context.Followers
                        .Where(f => f.UserId == currentUserId)
                        .Join(
                            context.Users,
                            f => f.FollowerId,
                            u => u.ChefId,
                            (f, u) => new UserFollowerDto
                            {
                                ChefId = u.ChefId,
                                UserName = u.UserName,
                                AvatarUrl = u.AvatarUrl,
                                ShortBio = u.ShortBio,
                                Location = u.Location,
                                FollowersCount = u.FollowersCount,
                                FollowingCount = u.FollowingCount
                            })
        .ToListAsync();
        }

        public async Task<List<UserFollowerDto>> GetFollowing(int currentUserId)
        {
            return await context.Followers
                .Where(f => f.FollowerId == currentUserId)
                .Join(
                    context.Users,
                    f => f.UserId,
                    u => u.ChefId,
                    (f, u) => new UserFollowerDto
                    {
                        ChefId = u.ChefId,
                        UserName = u.UserName,
                        AvatarUrl = u.AvatarUrl,
                        ShortBio = u.ShortBio,
                        Location = u.Location,
                        FollowersCount = u.FollowersCount,
                        FollowingCount = u.FollowingCount
                    })
                .ToListAsync();
        }

        public async Task<string?> DeleteFollower(FollowerDto dto)
        {
            var follower = await context.Followers
        .FirstOrDefaultAsync(f =>
            f.UserId == dto.UserId &&
            f.FollowerId == dto.FollowerId);

            if (follower == null)
                return "Not following";

            context.Followers.Remove(follower);
            await context.SaveChangesAsync();


            return string.Empty; 
        }

        public async Task<string?> InsertFollower(FollowerDto dto)
        {
            var follower = new Follower(); 
            follower.UserId = dto.UserId;
            follower.FollowerId = dto.FollowerId;

            context.Followers.Add(follower);
            await context.SaveChangesAsync(); 
            
            return string.Empty; 
        }

        public async Task<List<User>> GetUsers(int page, int pageSize)
        {
            return await context.Users
                .Where(u => u.IsSetup) 
                .ToListAsync(); 
        }

        public async Task<List<User>> SearchUsers(UserDto dto)
        {
            return null; 
        }

        public async Task<User?> GetUser(string user)
        {
            var foundUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == user);

            if (foundUser is not null)
            {
                return foundUser; 
            }

            return new User(); 
        }

        public async Task SetupProfile(UserDto user)
        {
            var foundUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == user.Username); 
            
            if (foundUser is not null)
            {
                foundUser.RealName = user.RealName;
                foundUser.Language = user.Language; 
                foundUser.ShortBio = user.ShortBio;
                foundUser.Location = user.Location;
                foundUser.FollowersCount = 0;
                foundUser.FollowingCount = 0;
                foundUser.Email = user.Email;
                foundUser.IsSetup = true;
                foundUser.AvatarUrl = user.AvatarUrl;  

                await context.SaveChangesAsync(); 
            }
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName), 
                new Claim(ClaimTypes.NameIdentifier, user.ChefId.ToString())
            };

            var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public async Task<User?> RegisterAsync(UserDto request)
        {
            if (await context.Users.AnyAsync(u => u.UserName == request.Username))
            {
                return new User(); 
            } 

            using (var context = new RecipeEntitiesContext())
            {
                var favorite = new Favorite
                {
                    Favorite1 = "No"  
                };
                context.Favorites.Add(favorite);
                await context.SaveChangesAsync();  

                var user = new User();  

                var hashedPassword = new PasswordHasher<User>().HashPassword(user, request.Password);
                user.UserName = request.Username;
                user.UserPassword = hashedPassword;

                var mealType = new MealType
                {
                    MealType1 = "User2"
                };

                context.MealTypes.Add(mealType);
                await context.SaveChangesAsync();  

                // hidden not pulled from db
                var category = new Category
                {
                    Category1 = "User2"
                }; 

                context.Categories.Add(category);
                await context.SaveChangesAsync();  

                var recipe = new Recipe
                {
                    DishId = mealType.DishId,
                    CategoryId = category.CategoryId,
                    FavoriteId = favorite.FavoriteId,
                    Chef = user,
                    RecipeName = "Example User2",
                    RecipeDescription = "Test",
                    IsSetupRecipe = true, 
                    RecipeIngredients = new List<RecipeIngredient>
                { 
                }
                };

                context.Recipes.Add(recipe);
                await context.SaveChangesAsync();
            } 

            return new User(); 
        }
    }
}
