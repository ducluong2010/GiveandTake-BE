using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Interfaces
{
    public interface INotificationService
    {
        Task<IGiveandtakeResult> GetNotificationById(int notificationId);
        Task<IGiveandtakeResult> GetAllNotifications(int page = 1, int pageSize = 8);
        Task<IGiveandtakeResult> CreateNotification(NotificationCreateDTO notificationInfo);
        Task<IGiveandtakeResult> UpdateNotification(int notificationId, NotificationUpdateDTO notificationInfo);
        Task<IGiveandtakeResult> DeleteNotification(int notificationId);
        Task<IGiveandtakeResult> ToggleIsReadStatus(int notificationId);
    }
}

