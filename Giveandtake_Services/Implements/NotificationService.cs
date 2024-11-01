using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Notification;
using Giveandtake_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Implements
{
    public class NotificationService : INotificationService
    {
        private readonly NotificationBusiness _notificationBusiness;

        public NotificationService()
        {
            _notificationBusiness = new NotificationBusiness();
        }

        public Task<IGiveandtakeResult> GetNotificationById(int notificationId)
            => _notificationBusiness.GetNotificationById(notificationId);

        public Task<IGiveandtakeResult> GetAllNotifications(int page = 1, int pageSize = 8)
            => _notificationBusiness.GetAllNotifications(page, pageSize);

        public Task<IGiveandtakeResult> CreateNotification(NotificationCreateDTO notificationInfo)
            => _notificationBusiness.CreateNotification(notificationInfo);

        public Task<IGiveandtakeResult> UpdateNotification(int notificationId, NotificationUpdateDTO notificationInfo)
            => _notificationBusiness.UpdateNotification(notificationId, notificationInfo);

        public Task<IGiveandtakeResult> DeleteNotification(int notificationId)
            => _notificationBusiness.DeleteNotification(notificationId);

        public Task<IGiveandtakeResult> ToggleIsReadStatus(int notificationId)
            => _notificationBusiness.ToggleIsReadStatus(notificationId);

        public Task<IGiveandtakeResult> GetAllNotificationsByAccountId(int accountId, int page = 1, int pageSize = 8)
           => _notificationBusiness.GetAllNotificationsByAccountId(accountId, page, pageSize);

        public Task<IGiveandtakeResult> GetAllNotificationsByStaffId(int staffId, int page = 1, int pageSize = 8)
            => _notificationBusiness.GetAllNotificationsByStaffId(staffId, page, pageSize);

        public Task<IGiveandtakeResult> GetNotiApprovedAccount(int id, int page = 1, int pageSize = 8)
            => _notificationBusiness.GetNotiApprovedAccount(id, page, pageSize);

        public Task<IGiveandtakeResult> GetNotiBonusAccount(int id, int page = 1, int pageSize = 8)
            => _notificationBusiness.GetNotiBonusAccount(id, page, pageSize);

        public Task<IGiveandtakeResult> GetNotiPointAccount(int id, int page = 1, int pageSize = 8)
            => _notificationBusiness.GetNotiPointAccount(id, page, pageSize);

        public Task<IGiveandtakeResult> GetNotiRejectAccount(int id, int page = 1, int pageSize = 8)
            => _notificationBusiness.GetNotiRejectAccount(id, page, pageSize);

        public Task<IGiveandtakeResult> GetNotiAcceptAccount(int id, int page = 1, int pageSize = 8)
            => _notificationBusiness.GetNotiAcceptAccount(id, page, pageSize);
    }
}
