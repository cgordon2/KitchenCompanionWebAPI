using KitchenCompanionWebApi.Models.DatabaseFirst; 
using KitchenCompanionWebApi.Models;
using KitchenCompanionWebApi.Models.DTOs;

namespace KitchenCompanionWebApi.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto request);
        Task<string?> LoginAsync(UserDto request);
        Task SetupProfile(UserDto request);
        Task<User?> GetUser(string request);
        Task<List<User>> SearchUsers(UserDto request); 
        Task<List<User>> GetUsers(int page, int pageSize);
        Task<string?> InsertFollower(FollowerDto dto);
        Task<string?> DeleteFollower(FollowerDto dto);

        Task<List<UserFollowerDto>> GetFollowers(int currentUserId);
        Task<List<UserFollowerDto>> GetFollowing(int currentUserId); 
    }
}
