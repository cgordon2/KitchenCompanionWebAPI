using KitchenCompanionWebApi.Models;
using KitchenCompanionWebApi.Models.DatabaseFirst;
using KitchenCompanionWebApi.Models.DTOs;
using KitchenCompanionWebApi.Services;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("ListUsers")]
        public async Task<ActionResult<List<User>>> GetUsers(string page)
        {
            var users = await authService.GetUsers(Convert.ToInt32(page), 20); 
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


        [HttpGet("GetUser")]
        public async Task<ActionResult<User>> GetUser([FromQuery] string userName)
        {
            Console.WriteLine($"QUERY userName = '{userName}'");
            Console.WriteLine($"ROUTE userName = '{HttpContext.Request.RouteValues["userName"]}'");

            var foundUser = await authService.GetUser(userName);
            return Ok(foundUser);
        }

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

            return Ok(token);
        }

        [Authorize]
        [HttpGet]
        public IActionResult TestAuthOnlyEndpoint()
        {
            return Ok("You can access this!"); 
        }
    }
}
