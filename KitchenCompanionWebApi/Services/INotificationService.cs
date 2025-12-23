using KitchenCompanionWebApi.Models.DTOs;

namespace KitchenCompanionWebApi.Services
{
    public interface INotificationService
    {
        Task<List<NotificationDto>> GetNotifications(int userId, bool isRead);
        Task<bool> MarkRead(List<NotificationDto> notificationList);
        Task<bool> CreateNotification(NotificationDto notificationDto); 
    }
}
