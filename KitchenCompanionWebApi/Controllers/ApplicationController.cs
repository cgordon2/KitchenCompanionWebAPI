using Microsoft.AspNetCore.Mvc;

namespace KitchenCompanionWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetApplicationDetails()
        {
            return Ok("WE DID IT"); 
        }
    }
}
