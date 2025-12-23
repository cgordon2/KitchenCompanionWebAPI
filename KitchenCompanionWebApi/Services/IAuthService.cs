using KitchenCompanionWebApi.Models.DatabaseFirst; 
using KitchenCompanionWebApi.Models;

namespace KitchenCompanionWebApi.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto request);
        Task<string?> LoginAsync(UserDto request);
        Task SetupProfile(UserDto request);
        Task<User?> GetUser(string request); 
    }
}
