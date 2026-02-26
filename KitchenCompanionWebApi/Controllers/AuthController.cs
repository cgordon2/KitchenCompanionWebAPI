using KitchenCompanionWebApi.Models;
using KitchenCompanionWebApi.Models.DatabaseFirst;
using KitchenCompanionWebApi.Models.DTOs;
using KitchenCompanionWebApi.Services;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace KitchenCompanionWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")] 
        public async Task<ActionResult<User>> Register(UserDto request)
        { 
            var user = await authService.RegisterAsync(request);
            if (user is null)
                return BadRequest("Username already exists");
            
           return Ok(user);
        }

        [HttpPost("Search")]
        public async Task<ActionResult<List<User>>> SearchUser()
        {
            return Ok(new List<User>()); 
        }

        [HttpGet("ListUsersWeb")]
        public async Task<List<User>> ListUsersWeb(int page = 1, int pageSize = 20)
        {
            var users = await authService.GetUsersWeb(page, pageSize);

            return users; 
        }

        [HttpGet("ListUsers")]
        public async Task<ActionResult<List<User>>> GetUsers(string page)
        {
            var users = await authService.GetUsers(Convert.ToInt32(page), 5); 
            return new OkObjectResult(users); 
        }

        [HttpGet("ListFollowing")]
        public async Task<ActionResult<List<UserFollowerDto>>> GetFollowing(int currentUserId)
        {
            var following = await authService.GetFollowing(currentUserId);

            return following; 
        }


        [HttpGet("ListFollowers")]
        public async Task<ActionResult<List<UserFollowerDto>>> GetFollowers(int currentUserId)
        {
            var followers = await authService.GetFollowers(currentUserId);

            return followers; 
        }

        [HttpPost("AddFollower")]
        public async Task<ActionResult<string>> InsertFollower(FollowerDto request)
        {
            await authService.InsertFollower(request);

            return "Success"; 
        }

        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
        [HttpGet("GetUser")]
        public async Task<ActionResult<User>> GetUser([FromQuery] string userName)
        {
            Console.WriteLine($"QUERY userName = '{userName}'");
            Console.WriteLine($"ROUTE userName = '{HttpContext.Request.RouteValues["userName"]}'");

            var foundUser = await authService.GetUser(userName);
            return Ok(foundUser);
        }

        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
        [HttpPost("CompleteProfile")]
        public async Task<ActionResult<User>> CompleteProfile(UserDto request)
        {
            await authService.SetupProfile(request);

            return Ok(request); 
        }

        [HttpPost("login")] 
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            var token = await authService.LoginAsync(request);

            if (token is null)
                return BadRequest("Invalid username or password");

            /**
             * TODO: REMOVE ME
             * **/
            Response.Cookies.Append("access_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new
            {
                success = true,
                token = token
            });
        }

        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
        [HttpGet("TEST")]
        public IActionResult TestAuthOnlyEndpoint()
        {
            return Ok(User.Identity?.Name);

        }
    }
}
