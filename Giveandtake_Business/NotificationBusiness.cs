using GiveandTake_Repo.DTOs.Notification;
using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Giveandtake_Business
{
    public class NotificationBusiness
    {
        private readonly UnitOfWork _unitOfWork;

        public NotificationBusiness()
        {
            _unitOfWork = new UnitOfWork();
        }

        public async Task<IGiveandtakeResult> GetAllNotifications(int page = 1, int pageSize = 8)
        {
            var notificationRepository = _unitOfWork.GetRepository<Notification>();
            var accountRepository = _unitOfWork.GetRepository<Account>();
            var donationRepository = _unitOfWork.GetRepository<Donation>(); 
            var allAccounts = await accountRepository.GetAllAsync();
            var allDonations = await donationRepository.GetAllAsync(); 

            var accountDict = allAccounts.ToDictionary(a => a.AccountId, a => a.FullName);
            var donationDict = allDonations.ToDictionary(d => d.DonationId, d => d.Name); 

            var allNotifications = await notificationRepository.GetListAsync(
                predicate: n => true,
                selector: n => new NotificationDTO
                {
                    NotificationId = n.NotificationId,
                    DonationId = n.DonationId,
                    DonationName = n.DonationId != null && donationDict.ContainsKey(n.DonationId.Value) 
                        ? donationDict[n.DonationId.Value]
                        : null,
                    AccountId = n.AccountId,
                    AccountName = n.AccountId.HasValue && accountDict.ContainsKey(n.AccountId.Value)
                        ? accountDict[n.AccountId.Value]
                        : null,
                    StaffId = n.StaffId,
                    StaffName = n.StaffId.HasValue && accountDict.ContainsKey(n.StaffId.Value)
                        ? accountDict[n.StaffId.Value]
                        : null,
                    Note = n.Note,
                    Type = n.Type,
                    CreatedAt = n.CreatedAt,
                    IsRead = n.IsRead
                }
            );

            int totalItems = allNotifications.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (page > totalPages) page = totalPages;
            if (totalItems == 0)
            {
                return new GiveandtakeResult(new PaginatedResult<NotificationDTO>
                {
                    Items = new List<NotificationDTO>(),
                    TotalItems = totalItems,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = totalPages
                });
            }

            var paginatedNotifications = allNotifications
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paginatedResult = new PaginatedResult<NotificationDTO>
            {
                Items = paginatedNotifications,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return new GiveandtakeResult(paginatedResult);
        }

        public async Task<IGiveandtakeResult> GetNotificationById(int notificationId)
        {
            var notificationRepository = _unitOfWork.GetRepository<Notification>();
            var accountRepository = _unitOfWork.GetRepository<Account>();
            var donationRepository = _unitOfWork.GetRepository<Donation>();

            var notification = await notificationRepository.SingleOrDefaultAsync(
                predicate: n => n.NotificationId == notificationId
            );

            if (notification == null)
            {
                return new GiveandtakeResult(-1, "Notification not found");
            }

            string accountName = null;
            if (notification.AccountId != null)
            {
                var account = await accountRepository.SingleOrDefaultAsync(
                    predicate: a => a.AccountId == notification.AccountId
                );
                accountName = account?.FullName;
            }

            string staffName = null;
            if (notification.StaffId != null)
            {
                var staffAccount = await accountRepository.SingleOrDefaultAsync(
                    predicate: a => a.AccountId == notification.StaffId
                );
                staffName = staffAccount?.FullName;
            }

            string donationName = null;
            if (notification.DonationId != null)
            {
                var donation = await donationRepository.SingleOrDefaultAsync(
                    predicate: d => d.DonationId == notification.DonationId
                );
                donationName = donation?.Name;
            }

            var notificationDTO = new NotificationDTO
            {
                NotificationId = notification.NotificationId,
                DonationId = notification.DonationId,
                DonationName = donationName, 
                AccountId = notification.AccountId,
                AccountName = accountName,
                StaffId = notification.StaffId,
                StaffName = staffName,
                Note = notification.Note,
                Type = notification.Type,
                CreatedAt = notification.CreatedAt,
                IsRead = notification.IsRead
            };

            return new GiveandtakeResult(notificationDTO);
        }

        public async Task<IGiveandtakeResult> CreateNotification(NotificationCreateDTO notificationInfo)
        {
            var accountRepository = _unitOfWork.GetRepository<Account>();
            var notificationRepository = _unitOfWork.GetRepository<Notification>();

            var accountExists = await accountRepository.SingleOrDefaultAsync(
                predicate: a => a.AccountId == notificationInfo.AccountId
            );

            if (accountExists == null)
            {
                return new GiveandtakeResult(-1, "Account not found");
            }

            var newNotification = new Notification
            {
                DonationId = notificationInfo.DonationId,
                AccountId = notificationInfo.AccountId,
                Note = notificationInfo.Note,
                Type = notificationInfo.Type,
                CreatedAt = DateTime.Now,
                IsRead = false,
                StaffId = notificationInfo.StaffId
            };

            await _unitOfWork.GetRepository<Notification>().InsertAsync(newNotification);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccessful)
            {
                return new GiveandtakeResult(-1, "Create notification unsuccessfully");
            }

            return new GiveandtakeResult(1, "Notification created successfully");
        }

        public async Task<IGiveandtakeResult> UpdateNotification(int id, NotificationUpdateDTO notificationInfo)
        {

            var existingNotification = await _unitOfWork.GetRepository<Notification>()
                .SingleOrDefaultAsync(predicate: n => n.NotificationId == id);

            if (existingNotification == null)
            {
                return new GiveandtakeResult(-1,"Notification not found");
            }

            if (!string.IsNullOrEmpty(notificationInfo.Note))
            {
                existingNotification.Note = notificationInfo.Note;
            }

            existingNotification.Type = string.IsNullOrEmpty(notificationInfo.Type)
                ? existingNotification.Type
                : notificationInfo.Type;
            _unitOfWork.GetRepository<Notification>().UpdateAsync(existingNotification);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            return isSuccessful
                ? new GiveandtakeResult(1, "Notification updated successfully")
                : new GiveandtakeResult(-1, "Update unsuccessfully");
        }

        public async Task<IGiveandtakeResult> DeleteNotification(int id)
        {
            var existingNotification = await _unitOfWork.GetRepository<Notification>()
                .SingleOrDefaultAsync(predicate: n => n.NotificationId == id);

            if (existingNotification == null)
            {
                return new GiveandtakeResult(-1, "Notification not found");
            }

            _unitOfWork.GetRepository<Notification>().DeleteAsync(existingNotification);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            return isSuccessful
                ? new GiveandtakeResult(1, "Notification deleted successfully")
                : new GiveandtakeResult(-1, "Delete unsuccessfully");
        }

        public async Task<IGiveandtakeResult> ToggleIsReadStatus(int id)
        {
            var existingNotification = await _unitOfWork.GetRepository<Notification>()
                .SingleOrDefaultAsync(predicate: n => n.NotificationId == id);

            if (existingNotification == null)
            {
                return new GiveandtakeResult(-1, "Notification not found");
            }
            existingNotification.IsRead = !existingNotification.IsRead;

            _unitOfWork.GetRepository<Notification>().UpdateAsync(existingNotification);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            return isSuccessful
                ? new GiveandtakeResult(1, "Notification read status updated successfully")
                : new GiveandtakeResult(-1, "Update unsuccessfully");
        }

        public async Task<IGiveandtakeResult> GetAllNotificationsByAccountId(int accountId, int page = 1, int pageSize = 8)
        {
            var notificationRepository = _unitOfWork.GetRepository<Notification>();
            var accountRepository = _unitOfWork.GetRepository<Account>();
            var donationRepository = _unitOfWork.GetRepository<Donation>();

            var allAccounts = await accountRepository.GetAllAsync();
            var allDonations = await donationRepository.GetAllAsync();

            var accountDict = allAccounts.ToDictionary(a => a.AccountId, a => a.FullName);
            var donationDict = allDonations.ToDictionary(d => d.DonationId, d => d.Name);

            var allNotifications = await notificationRepository.GetListAsync(
                predicate: n => n.AccountId == accountId && n.Type == "Cancel",
                selector: n => new NotificationDTO
                {
                    NotificationId = n.NotificationId,
                    DonationId = n.DonationId,
                    DonationName = n.DonationId != null && donationDict.ContainsKey(n.DonationId.Value)
                        ? donationDict[n.DonationId.Value]
                        : null,
                    AccountId = n.AccountId,
                    AccountName = accountDict.GetValueOrDefault(n.AccountId ?? 0),
                    StaffId = n.StaffId,
                    StaffName = accountDict.GetValueOrDefault(n.StaffId ?? 0),
                    Note = n.Note,
                    Type = n.Type,
                    CreatedAt = n.CreatedAt,
                    IsRead = n.IsRead
                }
            );

            var paginatedNotifications = allNotifications
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalItems = allNotifications.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var paginatedResult = new PaginatedResult<NotificationDTO>
            {
                Items = paginatedNotifications,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return new GiveandtakeResult(paginatedResult);
        }

        public async Task<IGiveandtakeResult> GetAllNotificationsByStaffId(int staffId, int page = 1, int pageSize = 8)
        {
            var accountRepository = _unitOfWork.GetRepository<Account>();
            var allAccounts = await accountRepository.GetAllAsync();
            var staffAccount = allAccounts.FirstOrDefault(a => a.AccountId == staffId);
            if (staffAccount == null || staffAccount.RoleId != 2)
            {
                return new GiveandtakeResult
                {
                    Message = "Invalid role,try again."
                };
            }

            var notificationRepository = _unitOfWork.GetRepository<Notification>();
            var donationRepository = _unitOfWork.GetRepository<Donation>();

            var allDonations = await donationRepository.GetAllAsync();
            var donationDict = allDonations.ToDictionary(d => d.DonationId, d => d.Name);
            var accountDict = allAccounts.ToDictionary(a => a.AccountId, a => a.FullName);

            var allNotifications = await notificationRepository.GetListAsync(
                predicate: n => n.StaffId == staffId && n.Type == "Pending",
                selector: n => new NotificationDTO
                {
                    NotificationId = n.NotificationId,
                    DonationId = n.DonationId,
                    DonationName = n.DonationId != null && donationDict.ContainsKey(n.DonationId.Value)
                        ? donationDict[n.DonationId.Value]
                        : null,
                    AccountId = n.AccountId,
                    AccountName = accountDict.GetValueOrDefault(n.AccountId ?? 0),
                    StaffId = n.StaffId,
                    StaffName = accountDict.GetValueOrDefault(n.StaffId ?? 0),
                    Note = n.Note,
                    Type = n.Type,
                    CreatedAt = n.CreatedAt,
                    IsRead = n.IsRead
                }
            );

            var paginatedNotifications = allNotifications
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalItems = allNotifications.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var paginatedResult = new PaginatedResult<NotificationDTO>
            {
                Items = paginatedNotifications,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return new GiveandtakeResult(paginatedResult);
        }

        public async Task<IGiveandtakeResult> GetNotiApprovedAccount(int id, int page = 1, int pageSize = 8)
        {
            var accountRepository = _unitOfWork.GetRepository<Account>();
            var allAccounts = await accountRepository.GetAllAsync();
            var notificationRepository = _unitOfWork.GetRepository<Notification>();
            var donationRepository = _unitOfWork.GetRepository<Donation>();

            var allDonations = await donationRepository.GetAllAsync();
            var donationDict = allDonations.ToDictionary(d => d.DonationId, d => d.Name);
            var accountDict = allAccounts.ToDictionary(a => a.AccountId, a => a.FullName);

            var allNotifications = await notificationRepository.GetListAsync(
                predicate: n => n.AccountId == id && n.Type == "Approved",
                selector: n => new NotificationDTO
                {
                    NotificationId = n.NotificationId,
                    DonationId = n.DonationId,
                    DonationName = n.DonationId != null && donationDict.ContainsKey(n.DonationId.Value)
                        ? donationDict[n.DonationId.Value]
                        : null,
                    AccountId = n.AccountId,
                    AccountName = accountDict.GetValueOrDefault(n.AccountId ?? 0),
                    StaffId = n.StaffId,
                    StaffName = accountDict.GetValueOrDefault(n.StaffId ?? 0),
                    Note = n.Note,
                    Type = n.Type,
                    CreatedAt = n.CreatedAt,
                    IsRead = n.IsRead
                }
            );

            var paginatedNotifications = allNotifications
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalItems = allNotifications.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var paginatedResult = new PaginatedResult<NotificationDTO>
            {
                Items = paginatedNotifications,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return new GiveandtakeResult(paginatedResult);
        }

        public async Task<IGiveandtakeResult> GetNotiBonusAccount(int id, int page = 1, int pageSize = 8)
        {
            var accountRepository = _unitOfWork.GetRepository<Account>();
            var allAccounts = await accountRepository.GetAllAsync();
            var notificationRepository = _unitOfWork.GetRepository<Notification>();
            var donationRepository = _unitOfWork.GetRepository<Donation>();

            var allDonations = await donationRepository.GetAllAsync();
            var donationDict = allDonations.ToDictionary(d => d.DonationId, d => d.Name);
            var accountDict = allAccounts.ToDictionary(a => a.AccountId, a => a.FullName);

            var allNotifications = await notificationRepository.GetListAsync(
                predicate: n => n.AccountId == id && n.Type == "Bonus",
                selector: n => new NotificationDTO
                {
                    NotificationId = n.NotificationId,
                    DonationId = n.DonationId,
                    DonationName = n.DonationId != null && donationDict.ContainsKey(n.DonationId.Value)
                        ? donationDict[n.DonationId.Value]
                        : null,
                    AccountId = n.AccountId,
                    AccountName = accountDict.GetValueOrDefault(n.AccountId ?? 0),
                    StaffId = n.StaffId,
                    StaffName = accountDict.GetValueOrDefault(n.StaffId ?? 0),
                    Note = n.Note,
                    Type = n.Type,
                    CreatedAt = n.CreatedAt,
                    IsRead = n.IsRead
                }
            );

            var paginatedNotifications = allNotifications
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalItems = allNotifications.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var paginatedResult = new PaginatedResult<NotificationDTO>
            {
                Items = paginatedNotifications,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return new GiveandtakeResult(paginatedResult);
        }

        public async Task<IGiveandtakeResult> GetNotiPointAccount(int id, int page = 1, int pageSize = 8)
        {
            var accountRepository = _unitOfWork.GetRepository<Account>();
            var allAccounts = await accountRepository.GetAllAsync();
            var notificationRepository = _unitOfWork.GetRepository<Notification>();
            var donationRepository = _unitOfWork.GetRepository<Donation>();

            var allDonations = await donationRepository.GetAllAsync();
            var donationDict = allDonations.ToDictionary(d => d.DonationId, d => d.Name);
            var accountDict = allAccounts.ToDictionary(a => a.AccountId, a => a.FullName);

            var allNotifications = await notificationRepository.GetListAsync(
                predicate: n => n.AccountId == id && n.Type == "Point",
                selector: n => new NotificationDTO
                {
                    NotificationId = n.NotificationId,
                    DonationId = n.DonationId,
                    DonationName = n.DonationId != null && donationDict.ContainsKey(n.DonationId.Value)
                        ? donationDict[n.DonationId.Value]
                        : null,
                    AccountId = n.AccountId,
                    AccountName = accountDict.GetValueOrDefault(n.AccountId ?? 0),
                    StaffId = n.StaffId,
                    StaffName = accountDict.GetValueOrDefault(n.StaffId ?? 0),
                    Note = n.Note,
                    Type = n.Type,
                    CreatedAt = n.CreatedAt,
                    IsRead = n.IsRead
                }
            );

            var paginatedNotifications = allNotifications
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalItems = allNotifications.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var paginatedResult = new PaginatedResult<NotificationDTO>
            {
                Items = paginatedNotifications,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return new GiveandtakeResult(paginatedResult);
        }

        public async Task<IGiveandtakeResult> GetNotiRejectAccount(int id, int page = 1, int pageSize = 8)
        {
            var accountRepository = _unitOfWork.GetRepository<Account>();
            var allAccounts = await accountRepository.GetAllAsync();
            var notificationRepository = _unitOfWork.GetRepository<Notification>();
            var donationRepository = _unitOfWork.GetRepository<Donation>();

            var allDonations = await donationRepository.GetAllAsync();
            var donationDict = allDonations.ToDictionary(d => d.DonationId, d => d.Name);
            var accountDict = allAccounts.ToDictionary(a => a.AccountId, a => a.FullName);

            var allNotifications = await notificationRepository.GetListAsync(
                predicate: n => n.AccountId == id && n.Type == "Reject",
                selector: n => new NotificationDTO
                {
                    NotificationId = n.NotificationId,
                    DonationId = n.DonationId,
                    DonationName = n.DonationId != null && donationDict.ContainsKey(n.DonationId.Value)
                        ? donationDict[n.DonationId.Value]
                        : null,
                    AccountId = n.AccountId,
                    AccountName = accountDict.GetValueOrDefault(n.AccountId ?? 0),
                    StaffId = n.StaffId,
                    StaffName = accountDict.GetValueOrDefault(n.StaffId ?? 0),
                    Note = n.Note,
                    Type = n.Type,
                    CreatedAt = n.CreatedAt,
                    IsRead = n.IsRead
                }
            );

            var paginatedNotifications = allNotifications
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalItems = allNotifications.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var paginatedResult = new PaginatedResult<NotificationDTO>
            {
                Items = paginatedNotifications,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return new GiveandtakeResult(paginatedResult);
        }

        public async Task<IGiveandtakeResult> GetNotiAcceptAccount(int id, int page = 1, int pageSize = 8)
        {
            var accountRepository = _unitOfWork.GetRepository<Account>();
            var allAccounts = await accountRepository.GetAllAsync();
            var notificationRepository = _unitOfWork.GetRepository<Notification>();
            var donationRepository = _unitOfWork.GetRepository<Donation>();

            var allDonations = await donationRepository.GetAllAsync();
            var donationDict = allDonations.ToDictionary(d => d.DonationId, d => d.Name);
            var accountDict = allAccounts.ToDictionary(a => a.AccountId, a => a.FullName);

            var allNotifications = await notificationRepository.GetListAsync(
                predicate: n => n.AccountId == id && n.Type == "Accept",
                selector: n => new NotificationDTO
                {
                    NotificationId = n.NotificationId,
                    DonationId = n.DonationId,
                    DonationName = n.DonationId != null && donationDict.ContainsKey(n.DonationId.Value)
                        ? donationDict[n.DonationId.Value]
                        : null,
                    AccountId = n.AccountId,
                    AccountName = accountDict.GetValueOrDefault(n.AccountId ?? 0),
                    StaffId = n.StaffId,
                    StaffName = accountDict.GetValueOrDefault(n.StaffId ?? 0),
                    Note = n.Note,
                    Type = n.Type,
                    CreatedAt = n.CreatedAt,
                    IsRead = n.IsRead
                }
            );

            var paginatedNotifications = allNotifications
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalItems = allNotifications.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var paginatedResult = new PaginatedResult<NotificationDTO>
            {
                Items = paginatedNotifications,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return new GiveandtakeResult(paginatedResult);
        }
    }
}
