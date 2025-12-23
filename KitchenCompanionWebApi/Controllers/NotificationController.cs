using KitchenCompanionWebApi.Models.DTOs;
using KitchenCompanionWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KitchenCompanionWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController(INotificationService notificationService) : ControllerBase
    {
        [HttpPost("Create")]

        public async Task<ActionResult<bool>> CreateNotification(NotificationDto dto)
        {
            bool result = await notificationService.CreateNotification(dto);

            return Ok(result);  
        }

        [HttpGet("Unread")]
        public async Task<ActionResult<NotificationDto>> GetUnreadNotifications(int userId)
        {
            var notifications = await notificationService.GetNotifications(userId, false);

            return new OkObjectResult(notifications); 
        }

        [HttpGet("Read")]
        public async Task<ActionResult<NotificationDto>> GetReadNotifications(int userId)
        {
            var notifications = await notificationService.GetNotifications(userId, true);

            return new OkObjectResult(notifications);
        }


        [HttpPut("MarkRead")]
        public async Task<ActionResult<bool>> MarkRead([FromBody] List<NotificationDto> dto)
        {
            bool done = await notificationService.MarkRead(dto);

            return Ok(done); 
        }
    }
}
