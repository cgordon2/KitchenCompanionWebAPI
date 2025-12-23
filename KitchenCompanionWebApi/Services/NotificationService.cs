using KitchenCompanionWebApi.Models.DatabaseFirst;
using KitchenCompanionWebApi.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KitchenCompanionWebApi.Services
{
    public class NotificationService : INotificationService
    {
        private readonly RecipeEntitiesContext _recipeEntitiesContext; 

        public NotificationService(RecipeEntitiesContext recipeEntitiesContext)
        {
            _recipeEntitiesContext = recipeEntitiesContext;
        }
         
        public async Task<bool> CreateNotification(NotificationDto notificationDto)
        {
            try
            {
                var notification = new Notification
                {
                    UserId = notificationDto.UserId,
                    Message = notificationDto.Message,
                    IsRead = notificationDto.IsRead,
                    CreatedAt = notificationDto.CreatedAt
                };

                _recipeEntitiesContext.Notifications.Add(notification);
                await _recipeEntitiesContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            { 
                return false;
            }
        } 

        public async Task<List<NotificationDto>> GetNotifications(int userId, bool isRead)
        {
            if (!isRead)
            {
                var notifications = await (from n in _recipeEntitiesContext.Notifications
                                           where n.UserId == userId && n.IsRead == false
                                           select new NotificationDto
                                           {
                                               NotificationId = n.NotifId,
                                               UserId = n.UserId,
                                               Message = n.Message,
                                               IsRead = n.IsRead,
                                               CreatedAt = n.CreatedAt
                                               
                                           })
                           .ToListAsync();

                return notifications;
            }
            else
            {
                var notifications = await (from n in _recipeEntitiesContext.Notifications
                                           where n.UserId == userId && n.IsRead == true
                                           select new NotificationDto
                                           {
                                               NotificationId = n.NotifId,
                                               UserId = n.UserId,
                                               Message = n.Message,
                                               IsRead = n.IsRead,
                                               CreatedAt = n.CreatedAt 
                                           })
                                            .ToListAsync();

                return notifications;
            }
        } 

        public async Task<bool> MarkRead(List<NotificationDto> notificationList)
        {
            try
            { 
                var ids = notificationList.Select(n => n.NotificationId).ToList();
                 
                var notifications = await _recipeEntitiesContext.Notifications
                    .Where(n => ids.Contains(n.NotifId))
                    .ToListAsync();
                 
                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                } 
                await _recipeEntitiesContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            { 
                return false;
            }
        }
    }
}
