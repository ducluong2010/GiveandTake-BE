using GiveandTake_Repo.DTOs.Account;
using GiveandTake_Repo.DTOs.Donation;
using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GiveandTake_Repo.DTOs.Feedback;
using Microsoft.AspNetCore.Http;


namespace Giveandtake_Business
{
    public class DonationBusiness
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DonationBusiness(IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = new UnitOfWork();
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IGiveandtakeResult> GetAllDonations(int page = 1, int pageSize = 8)
        {
            var repository = _unitOfWork.GetRepository<Donation>();
            var accountRepository = _unitOfWork.GetRepository<Account>();

            var allAccounts = await accountRepository.GetAllAsync();
            var accountDict = allAccounts.ToDictionary(a => a.AccountId, a => a.FullName);

            var allDonations = await repository.GetListAsync(
                predicate: d => true,
                selector: d => new DonationDTO
                {
                    DonationId = d.DonationId,
                    AccountId = d.AccountId,
                    AccountName = d.Account.FullName,
                    CategoryId = d.CategoryId,
                    CategoryName = d.Category.CategoryName,
                    Name = d.Name,
                    Description = d.Description,
                    Point = d.Point,
                    Type = d.Type,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,
                    ApprovedBy = d.ApprovedBy,
                    ApprovedByName = d.ApprovedBy.HasValue && accountDict.ContainsKey(d.ApprovedBy.Value)
                        ? accountDict[d.ApprovedBy.Value]
                        : null,
                    TotalRating = d.TotalRating,
                    Status = d.Status,
                    DonationImages = d.DonationImages.Select(di => di.Url).ToList(),
                    Feedbacks = d.Feedbacks.Select(f => new FeedbackDTO
                    {
                        FeedbackId = f.FeedbackId,
                        SenderId = f.SenderId,
                        SenderName = f.SenderId.HasValue && accountDict.ContainsKey(f.SenderId.Value)
                                    ? accountDict[f.SenderId.Value] 
                                    : null,
                        AccountId = f.AccountId,
                        AccountName = f.Account.FullName,
                        DonationId = d.DonationId,
                        DonationName = d.Name,
                        Rating = f.Rating,
                        Content = f.Content,
                        CreatedDate = f.CreatedDate,
                        FeedbackMediaUrls = f.FeedbackMedia.Select(fm => fm.MediaUrl).ToList() 
                    }).ToList()
                },
                include: source => source
                    .Include(d => d.Account)
                    .Include(d => d.Category)
                    .Include(d => d.Feedbacks) 
            );

            int totalItems = allDonations.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (page > totalPages) page = totalPages;
            if (totalItems == 0)
            {
                return new GiveandtakeResult(new PaginatedResult<DonationDTO>
                {
                    Items = new List<DonationDTO>(),
                    TotalItems = totalItems,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = totalPages
                });
            }

            var paginatedDonations = allDonations
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paginatedResult = new PaginatedResult<DonationDTO>
            {
                Items = paginatedDonations,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return new GiveandtakeResult(paginatedResult);
        }

        public async Task<IGiveandtakeResult> GetAllByStaff(int accountId, int page = 1, int pageSize = 8)
        {
            var accountRepository = _unitOfWork.GetRepository<Account>();
            var donationRepository = _unitOfWork.GetRepository<Donation>();
            var account = await accountRepository.GetAllAsync(a => a.AccountId == accountId && a.RoleId == 2)
                                                 .ContinueWith(t => t.Result.FirstOrDefault());
            if (account == null)
            {
                return new GiveandtakeResult(-1, "Account not found or not a staff member");
            }

            int accountActiveTime = account.ActiveTime ?? -1; 
            TimeSpan startTime, endTime;

            if (accountActiveTime == 1)
            {
                startTime = TimeSpan.Zero; 
                endTime = new TimeSpan(12, 0, 0); 
            }
            else if (accountActiveTime == 2)
            {
                startTime = new TimeSpan(12, 0, 0);
                endTime = new TimeSpan(24, 0, 0); 
            }
            else
            {
                return new GiveandtakeResult(-1, "Invalid ActiveTime value in account");
            }

            var allAccounts = await accountRepository.GetAllAsync();
            var accountDict = allAccounts.ToDictionary(a => a.AccountId, a => a.FullName);

            var allDonations = await donationRepository.GetListAsync(
                predicate: d => d.CreatedAt.HasValue &&
                                d.CreatedAt.Value.TimeOfDay >= startTime &&
                                d.CreatedAt.Value.TimeOfDay < endTime &&
                                (!d.ApprovedBy.HasValue || d.ApprovedBy == accountId),
                selector: d => new DonationDTO
                {
                    DonationId = d.DonationId,
                    AccountId = d.AccountId,
                    AccountName = d.Account.FullName,
                    CategoryId = d.CategoryId,
                    CategoryName = d.Category.CategoryName,
                    Name = d.Name,
                    Description = d.Description,
                    Point = d.Point,
                    Type = d.Type,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,
                    ApprovedBy = d.ApprovedBy,
                    ApprovedByName = d.ApprovedBy.HasValue && accountDict.ContainsKey(d.ApprovedBy.Value)
                        ? accountDict[d.ApprovedBy.Value]
                        : null,
                    TotalRating = d.TotalRating,
                    Status = d.Status,
                    DonationImages = d.DonationImages.Select(di => di.Url).ToList(),
                    Feedbacks = d.Feedbacks.Select(f => new FeedbackDTO
                    {
                        FeedbackId = f.FeedbackId,
                        SenderId = f.SenderId,
                        SenderName = f.SenderId.HasValue && accountDict.ContainsKey(f.SenderId.Value)
                                    ? accountDict[f.SenderId.Value]
                                    : null,
                        AccountId = f.AccountId,
                        AccountName = f.Account.FullName,
                        DonationId = d.DonationId,
                        DonationName = d.Name,
                        Rating = f.Rating,
                        Content = f.Content,
                        CreatedDate = f.CreatedDate,
                        FeedbackMediaUrls = f.FeedbackMedia.Select(fm => fm.MediaUrl).ToList()
                    }).ToList()
                },
                include: source => source
                    .Include(d => d.Account)
                    .Include(d => d.Category)
                    .Include(d => d.Feedbacks)
            );

            var sortedDonations = allDonations.OrderByDescending(d => d.CreatedAt).ToList();

            int totalItems = sortedDonations.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (page > totalPages) page = totalPages;
            if (totalItems == 0)
            {
                return new GiveandtakeResult(new PaginatedResult<DonationDTO>
                {
                    Items = new List<DonationDTO>(),
                    TotalItems = totalItems,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = totalPages
                });
            }

            var paginatedDonations = sortedDonations
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paginatedResult = new PaginatedResult<DonationDTO>
            {
                Items = paginatedDonations,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return new GiveandtakeResult(paginatedResult);
        }

        public async Task<IGiveandtakeResult> GetAllByStaffV2(int accountId, int page = 1, int pageSize = 8)
        {
            var accountRepository = _unitOfWork.GetRepository<Account>();
            var donationRepository = _unitOfWork.GetRepository<Donation>();

            var account = await accountRepository.GetAllAsync(a => a.AccountId == accountId && a.RoleId == 2)
                                                 .ContinueWith(t => t.Result.FirstOrDefault());
            if (account == null)
            {
                return new GiveandtakeResult(-1, "Account không tồn tại hoặc không phải staff");
            }

            var categoryId = account.CategoryId;
            if (!categoryId.HasValue)
            {
                return new GiveandtakeResult(-1, "Bạn chưa được phân công công việc.");
            }

            var allAccounts = await accountRepository.GetAllAsync();
            var accountDict = allAccounts.ToDictionary(a => a.AccountId, a => a.FullName);

            var allDonations = await donationRepository.GetListAsync(
                predicate: d => d.CategoryId == categoryId.Value &&
                                (!d.ApprovedBy.HasValue || d.ApprovedBy == accountId || d.Status == "Pending"),
                selector: d => new DonationDTO
                {
                    DonationId = d.DonationId,
                    AccountId = d.AccountId,
                    AccountName = d.Account.FullName,
                    CategoryId = d.CategoryId,
                    CategoryName = d.Category.CategoryName,
                    Name = d.Name,
                    Description = d.Description,
                    Point = d.Point,
                    Type = d.Type,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,
                    ApprovedBy = d.ApprovedBy,
                    ApprovedByName = d.ApprovedBy.HasValue && accountDict.ContainsKey(d.ApprovedBy.Value)
                        ? accountDict[d.ApprovedBy.Value]
                        : null,
                    TotalRating = d.TotalRating,
                    Status = d.Status,
                    DonationImages = d.DonationImages.Select(di => di.Url).ToList(),
                    Feedbacks = d.Feedbacks.Select(f => new FeedbackDTO
                    {
                        FeedbackId = f.FeedbackId,
                        SenderId = f.SenderId,
                        SenderName = f.SenderId.HasValue && accountDict.ContainsKey(f.SenderId.Value)
                                    ? accountDict[f.SenderId.Value]
                                    : null,
                        AccountId = f.AccountId,
                        AccountName = f.Account.FullName,
                        DonationId = d.DonationId,
                        DonationName = d.Name,
                        Rating = f.Rating,
                        Content = f.Content,
                        CreatedDate = f.CreatedDate,
                        FeedbackMediaUrls = f.FeedbackMedia.Select(fm => fm.MediaUrl).ToList()
                    }).ToList()
                },
                include: source => source
                    .Include(d => d.Account)
                    .Include(d => d.Category)
                    .Include(d => d.Feedbacks)
            );

            var sortedDonations = allDonations.OrderByDescending(d => d.CreatedAt).ToList();

            int totalItems = sortedDonations.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (page > totalPages) page = totalPages;
            if (totalItems == 0)
            {
                return new GiveandtakeResult(new PaginatedResult<DonationDTO>
                {
                    Items = new List<DonationDTO>(),
                    TotalItems = totalItems,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = totalPages
                });
            }

            var paginatedDonations = sortedDonations
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paginatedResult = new PaginatedResult<DonationDTO>
            {
                Items = paginatedDonations,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return new GiveandtakeResult(paginatedResult);
        }

        public async Task<IGiveandtakeResult> GetDonationsByAccountId(int accountId)
        {
            if (accountId <= 0)
            {
                return new GiveandtakeResult(-1, "Invalid account ID.");
            }

            var accountRepository = _unitOfWork.GetRepository<Account>();
            var donationRepository = _unitOfWork.GetRepository<Donation>();


            var accountExists = await accountRepository.GetAllAsync(a => a.AccountId == accountId);
            if (!accountExists.Any())
            {
                return new GiveandtakeResult(-1, "Account not found.");
            }
            var allAccounts = await accountRepository.GetAllAsync();
            var accountDict = allAccounts.ToDictionary(a => a.AccountId, a => a.FullName);

            var claimedDonations = await donationRepository.GetListAsync(
                predicate: d => d.AccountId == accountId && d.Status == "Claimed",
                selector: d => new DonationDTO
                {
                    DonationId = d.DonationId,
                    AccountId = d.AccountId,
                    AccountName = d.Account.FullName, 
                    CategoryId = d.CategoryId,
                    CategoryName = d.Category.CategoryName,
                    Name = d.Name,
                    Description = d.Description,
                    Point = d.Point,
                    Type = d.Type,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,
                    ApprovedBy = d.ApprovedBy,
                    ApprovedByName = d.ApprovedBy.HasValue ? d.ApprovedBy.ToString() : null,
                    TotalRating = d.TotalRating,
                    Status = d.Status,
                    DonationImages = d.DonationImages.Select(di => di.Url).ToList(),
                    Feedbacks = d.Feedbacks.Select(f => new FeedbackDTO
                    {
                        FeedbackId = f.FeedbackId,
                        SenderId = f.SenderId,
                        SenderName = f.SenderId.HasValue ? f.SenderId.ToString() : null,
                        AccountId = f.AccountId,
                        AccountName = f.Account.FullName,
                        DonationId = d.DonationId,
                        DonationName = d.Name,
                        Rating = f.Rating,
                        Content = f.Content,
                        CreatedDate = f.CreatedDate,
                        FeedbackMediaUrls = f.FeedbackMedia.Select(fm => fm.MediaUrl).ToList()
                    }).ToList()
                },
                include: source => source
                    .Include(d => d.Account)
                    .Include(d => d.Category)
                    .Include(d => d.Feedbacks)
            );

            var sortedDonations = claimedDonations.OrderByDescending(d => d.CreatedAt).ToList();

            return new GiveandtakeResult(sortedDonations);
        }

        public async Task<IGiveandtakeResult> GetAllByAccountId(int accountId)
        {
            if (accountId <= 0) 
            {
                return new GiveandtakeResult(-1, "Invalid account ID.");
            }

            var accountRepository = _unitOfWork.GetRepository<Account>();
            var donationRepository = _unitOfWork.GetRepository<Donation>();

           
            var accountExists = await accountRepository.GetAllAsync(a => a.AccountId == accountId);
            if (!accountExists.Any())
            {
                return new GiveandtakeResult(-1, "Account not found.");
            }
            var allAccounts = await accountRepository.GetAllAsync();
            var accountDict = allAccounts.ToDictionary(a => a.AccountId, a => a.FullName);

            var claimedDonations = await donationRepository.GetListAsync(
                predicate: d => d.AccountId == accountId,
                selector: d => new DonationDTO
                {
                    DonationId = d.DonationId,
                    AccountId = d.AccountId,
                    AccountName = d.Account.FullName,
                    CategoryId = d.CategoryId,
                    CategoryName = d.Category.CategoryName,
                    Name = d.Name,
                    Description = d.Description,
                    Point = d.Point,
                    Type = d.Type,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,
                    ApprovedBy = d.ApprovedBy,
                    ApprovedByName = d.ApprovedBy.HasValue ? d.ApprovedBy.ToString() : null,
                    TotalRating = d.TotalRating,
                    Status = d.Status,
                    DonationImages = d.DonationImages.Select(di => di.Url).ToList(),
                    Feedbacks = d.Feedbacks.Select(f => new FeedbackDTO
                    {
                        FeedbackId = f.FeedbackId,
                        SenderId = f.SenderId,
                        SenderName = f.SenderId.HasValue ? f.SenderId.ToString() : null,
                        AccountId = f.AccountId,
                        AccountName = f.Account.FullName,
                        DonationId = d.DonationId,
                        DonationName = d.Name,
                        Rating = f.Rating,
                        Content = f.Content,
                        CreatedDate = f.CreatedDate,
                        FeedbackMediaUrls = f.FeedbackMedia.Select(fm => fm.MediaUrl).ToList()
                    }).ToList()
                },
                include: source => source
                    .Include(d => d.Account)
                    .Include(d => d.Category)
                    .Include(d => d.Feedbacks)
            );

            var sortedDonations = claimedDonations.OrderByDescending(d => d.CreatedAt).ToList();

            return new GiveandtakeResult(sortedDonations);
        }

        public async Task<IGiveandtakeResult> GetByAccountIdAndType(int accountId, int type)
        {
            var donationRepository = _unitOfWork.GetRepository<Donation>();
            var donations = await donationRepository.GetListAsync(
                predicate: d => d.AccountId == accountId && d.Type == type && d.Status == "Approved",
                selector: d => new DonationDTO
                {
                    DonationId = d.DonationId,
                    AccountId = d.AccountId,
                    AccountName = d.Account.FullName, 
                    CategoryId = d.CategoryId,
                    CategoryName = d.Category.CategoryName,
                    Name = d.Name,
                    Description = d.Description,
                    Point = d.Point,
                    Type = d.Type,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,
                    ApprovedBy = d.ApprovedBy,
                    ApprovedByName = d.ApprovedBy.HasValue ? d.ApprovedBy.ToString() : null,
                    TotalRating = d.TotalRating,
                    Status = d.Status,
                    DonationImages = d.DonationImages.Select(di => di.Url).ToList(),
                    Feedbacks = d.Feedbacks.Select(f => new FeedbackDTO
                    {
                        FeedbackId = f.FeedbackId,
                        SenderId = f.SenderId,
                        SenderName = f.SenderId.HasValue ? f.SenderId.ToString() : null,
                        AccountId = f.AccountId,
                        AccountName = f.Account.FullName,
                        DonationId = d.DonationId,
                        DonationName = d.Name,
                        Rating = f.Rating,
                        Content = f.Content,
                        CreatedDate = f.CreatedDate,
                        FeedbackMediaUrls = f.FeedbackMedia.Select(fm => fm.MediaUrl).ToList()
                    }).ToList()
                },
                include: source => source
                    .Include(d => d.Account)
                    .Include(d => d.Category)
                    .Include(d => d.Feedbacks)
            );

            var sortedDonations = donations.OrderByDescending(d => d.CreatedAt).ToList();

            return new GiveandtakeResult(sortedDonations);
        }

        public async Task<IGiveandtakeResult> GetAllApproved(int page = 1, int pageSize = 8)
        {
            var repository = _unitOfWork.GetRepository<Donation>();
            var accountRepository = _unitOfWork.GetRepository<Account>();
            var allAccounts = await accountRepository.GetAllAsync();
            var accountDict = allAccounts.ToDictionary(a => a.AccountId, a => a.FullName);
            var allDonations = await repository.GetListAsync(
                predicate: d => d.ApprovedBy.HasValue, 
                selector: d => new DonationDTO
                {
                    DonationId = d.DonationId,
                    AccountId = d.AccountId,
                    AccountName = d.Account.FullName,
                    CategoryId = d.CategoryId,
                    CategoryName = d.Category.CategoryName,
                    Name = d.Name,
                    Description = d.Description,
                    Point = d.Point,
                    Type = d.Type,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,
                    ApprovedBy = d.ApprovedBy,
                    ApprovedByName = d.ApprovedBy.HasValue && accountDict.ContainsKey(d.ApprovedBy.Value)
                        ? accountDict[d.ApprovedBy.Value]
                        : null,
                    TotalRating = d.TotalRating,
                    Status = d.Status,
                    DonationImages = d.DonationImages.Select(di => di.Url).ToList(),
                    Feedbacks = d.Feedbacks.Select(f => new FeedbackDTO
                    {
                        FeedbackId = f.FeedbackId,
                        SenderId = f.SenderId,
                        SenderName = f.SenderId.HasValue && accountDict.ContainsKey(f.SenderId.Value)
                                    ? accountDict[f.SenderId.Value]
                                    : null,
                        AccountId = f.AccountId,
                        AccountName = f.Account.FullName,
                        DonationId = d.DonationId,
                        DonationName = d.Name,
                        Rating = f.Rating,
                        Content = f.Content,
                        CreatedDate = f.CreatedDate,
                        FeedbackMediaUrls = f.FeedbackMedia.Select(fm => fm.MediaUrl).ToList()
                    }).ToList()
                },
                include: source => source
                    .Include(d => d.Account)
                    .Include(d => d.Category)
                    .Include(d => d.Feedbacks)
            );

            var sortedDonations = allDonations.OrderByDescending(d => d.CreatedAt).ToList();

            int totalItems = sortedDonations.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (page > totalPages) page = totalPages;
            if (totalItems == 0)
            {
                return new GiveandtakeResult(new PaginatedResult<DonationDTO>
                {
                    Items = new List<DonationDTO>(),
                    TotalItems = totalItems,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = totalPages
                });
            }

            var paginatedDonations = sortedDonations
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paginatedResult = new PaginatedResult<DonationDTO>
            {
                Items = paginatedDonations,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return new GiveandtakeResult(paginatedResult);
        }

        public async Task<IGiveandtakeResult> GetDonationById(int donationId)
        {
            var donationRepository = _unitOfWork.GetRepository<Donation>();
            var accountRepository = _unitOfWork.GetRepository<Account>();

            var donation = await donationRepository.SingleOrDefaultAsync(
                predicate: d => d.DonationId == donationId,
                include: source => source
                    .Include(d => d.Account)
                    .Include(d => d.Category)
                    .Include(d => d.DonationImages)
                    .Include(d => d.Feedbacks) 
            );

            if (donation == null)
            {
                return new GiveandtakeResult(-1, "Donation not found");
            } 

            var feedbackAccountIds = donation.Feedbacks
                    .Where(f => f.AccountId.HasValue)
                    .Select(f => f.AccountId.Value)
                    .Distinct()
                    .ToList();

            var accounts = await accountRepository.GetAllAsync(
                predicate: a => feedbackAccountIds.Contains(a.AccountId)
            );

           
            var allAccounts = await accountRepository.GetAllAsync();
            var accountDict = allAccounts.ToDictionary(a => a.AccountId, a => a.FullName);

            string approverName = null;
            if (donation.ApprovedBy.HasValue)
            {
                var approver = await accountRepository.SingleOrDefaultAsync(
                    predicate: a => a.AccountId == donation.ApprovedBy.Value
                );
                approverName = approver?.FullName;
            }

            var donationDTO = new DonationDTO
            {
                DonationId = donation.DonationId,
                AccountId = donation.AccountId,
                AccountName = donation.Account?.FullName,
                CategoryId = donation.CategoryId,
                CategoryName = donation.Category?.CategoryName,
                Name = donation.Name,
                Description = donation.Description,
                Point = donation.Point,
                Type = donation.Type,
                CreatedAt = donation.CreatedAt,
                UpdatedAt = donation.UpdatedAt,
                ApprovedBy = donation.ApprovedBy,
                ApprovedByName = approverName,
                TotalRating = donation.TotalRating,
                Status = donation.Status,
                DonationImages = donation.DonationImages?.Select(di => di.Url).ToList() ?? new List<string>(),
                Feedbacks = donation.Feedbacks.Select(f => new FeedbackDTO
                {
                    FeedbackId = f.FeedbackId,
                    SenderId = f.SenderId, 
                    SenderName = f.SenderId.HasValue && accountDict.ContainsKey(f.SenderId.Value)
                                ? accountDict[f.SenderId.Value] 
                                : null,
                    AccountId = f.AccountId,
                    AccountName = f.AccountId.HasValue && accountDict.ContainsKey(f.AccountId.Value)
                                 ? accountDict[f.AccountId.Value]
                                 : "",
                    DonationId = f.DonationId, 
                    DonationName = f.Donation.Name,
                    Rating = f.Rating,
                    Content = f.Content,
                    CreatedDate = f.CreatedDate,
                    FeedbackMediaUrls = f.FeedbackMedia.Select(fm => fm.MediaUrl).ToList() 
                }).ToList()
            };

            return new GiveandtakeResult(donationDTO);
        }

        public async Task<IGiveandtakeResult> UpdateDonation(int id, CreateUpdateDonationDTO donationInfo)
        {
            var donationRepository = _unitOfWork.GetRepository<Donation>();
            var categoryRepository = _unitOfWork.GetRepository<Category>();

            var existingDonation = await donationRepository.SingleOrDefaultAsync(
                predicate: d => d.DonationId == id
            );

            if (existingDonation == null)
            {
                return new GiveandtakeResult(-1, "Donation not found");
            }

            if (donationInfo.CategoryId.HasValue)
            {
                var category = await categoryRepository.SingleOrDefaultAsync(
                    predicate: c => c.CategoryId == donationInfo.CategoryId.Value
                );
                if (category == null)
                {
                    return new GiveandtakeResult(-1, "Category not found");
                }
                existingDonation.CategoryId = category.CategoryId;
                existingDonation.Point = category.Point;
            }
            if (!string.IsNullOrEmpty(donationInfo.Name))
                existingDonation.Name = donationInfo.Name;

            if (!string.IsNullOrEmpty(donationInfo.Description))
                existingDonation.Description = donationInfo.Description;

            if (donationInfo.AccountId.HasValue)
                existingDonation.AccountId = donationInfo.AccountId.Value;

            if (donationInfo.ApprovedBy.HasValue)
                existingDonation.ApprovedBy = donationInfo.ApprovedBy.Value;

            if (donationInfo.TotalRating.HasValue)
                existingDonation.TotalRating = donationInfo.TotalRating.Value;

            if (donationInfo.Type.HasValue)
                existingDonation.Type = donationInfo.Type.Value;

            if (!string.IsNullOrEmpty(donationInfo.Status))
                existingDonation.Status = donationInfo.Status;

            existingDonation.UpdatedAt = DateTime.Now;

            if (donationInfo.DonationImages != null && donationInfo.DonationImages.Any())
            {
                await UpdateDonationImages(existingDonation.DonationId, donationInfo.DonationImages);
            }

            _unitOfWork.GetRepository<Donation>().UpdateAsync(existingDonation);
            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult(1, "Donation updated successfully");
        }

        private async Task UpdateDonationImages(int donationId, IEnumerable<string> donationImages)
        {
            var donationImageRepository = _unitOfWork.GetRepository<DonationImage>();

            var existingImages = await donationImageRepository.GetListAsync(
                predicate: di => di.DonationId == donationId
            );

            if (existingImages != null && existingImages.Any())
            {
                foreach (var image in existingImages)
                {
                    donationImageRepository.DeleteAsync(image); 
                }
            }

            foreach (var imageUrl in donationImages)
            {
                var newDonationImage = new DonationImage
                {
                    DonationId = donationId,
                    Url = imageUrl,
                    IsThumbnail = false 
                };
                await donationImageRepository.InsertAsync(newDonationImage); 
            }

            await _unitOfWork.CommitAsync();
        }

        public async Task<IGiveandtakeResult> CreateDonation(CreateDonationDTO donationInfo)
        {
            var category = await _unitOfWork.GetRepository<Category>()
                .FirstOrDefaultAsync(c => c.CategoryId == donationInfo.CategoryId);

            if (category == null)
            {
                return new GiveandtakeResult(-1, "Category not found");
            }

            var newDonation = new Donation
            {
                AccountId = donationInfo.AccountId,
                CategoryId = donationInfo.CategoryId,
                Name = donationInfo.Name,
                Description = donationInfo.Description,
                Point = category.Point,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Status = "Pending",
                Type = donationInfo.Type,
                DonationImages = donationInfo.DonationImages.Select(imageUrl => new DonationImage
                {
                    Url = imageUrl,
                    IsThumbnail = false 
                }).ToList()
            };

            await _unitOfWork.GetRepository<Donation>().InsertAsync(newDonation);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccessful)
            {
                return new GiveandtakeResult(-1, "Create donation unsuccessfully");
            }

            return new GiveandtakeResult(1, "Donation created successfully");
        }

        public async Task<IGiveandtakeResult> DeleteDonation(int id)
        {
            var donation = await _unitOfWork.GetRepository<Donation>()
                .FirstOrDefaultAsync(d => d.DonationId == id);


            if (donation == null)
            {
                return new GiveandtakeResult(-1, "Donation not found");
            }

            var donationImages = (await _unitOfWork.GetRepository<DonationImage>()
                .GetAllAsync()).Where(img => img.DonationId == id).ToList();

            foreach (var image in donationImages)
            {
                _unitOfWork.GetRepository<DonationImage>().DeleteAsync(image);
            }

            _unitOfWork.GetRepository<Donation>().DeleteAsync(donation);

            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult(1, "Donation deleted successfully");
        }

        public async Task<IGiveandtakeResult> ToggleDonationStatus(int donationId)
        {
            var donation = await _unitOfWork.GetRepository<Donation>()
                .SingleOrDefaultAsync(predicate: d => d.DonationId == donationId);

            if (donation == null)
            {
                return new GiveandtakeResult(-1, "Donation not found");
            }

            
            var accountIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("accountId");
            if (accountIdClaim == null)
            {
                return new GiveandtakeResult(-1, "Account ID not found in token.");
            }

            var accountId = int.TryParse(accountIdClaim.Value, out var id) ? id : (int?)null;

            if (donation.Status == "Pending")
            {
                donation.Status = "Approved";
                donation.ApprovedBy = accountId; 
            }
            else if (donation.Status == "Approved")
            {
                donation.Status = "Pending";
                donation.ApprovedBy = accountId;
            }
            else
            {
                return new GiveandtakeResult(-1, "Donation is in an invalid status.");
            }

            donation.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Donation>().UpdateAsync(donation);
            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult(1, "Donation status toggled successfully");
        }

        public async Task<IGiveandtakeResult> ToggleCancel(int donationId)
        {
            var donation = await _unitOfWork.GetRepository<Donation>()
                .SingleOrDefaultAsync(predicate: d => d.DonationId == donationId);

            if (donation == null)
            {
                return new GiveandtakeResult(-1, "Donation not found");
            }

            var accountIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("accountId");
            if (accountIdClaim == null)
            {
                return new GiveandtakeResult(-1, "Account ID not found in token.");
            }

            var accountId = int.TryParse(accountIdClaim.Value, out var id) ? id : (int?)null;

            if (donation.Status == "Pending")
            {
                donation.Status = "Cancel";
                donation.ApprovedBy = accountId;
            }
            else if (donation.Status == "Cancel")
            {
                donation.Status = "Pending";
                donation.ApprovedBy = accountId;
            }
            else
            {
                return new GiveandtakeResult(-1, "Donation is in an invalid status.");
            }
            donation.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Donation>().UpdateAsync(donation);
            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult(1, "Donation status toggled successfully");
        }

        public async Task<IGiveandtakeResult> ToggleApproved(int donationId)
        {
            var donation = await _unitOfWork.GetRepository<Donation>()
                .SingleOrDefaultAsync(predicate: d => d.DonationId == donationId);

            if (donation == null)
            {
                return new GiveandtakeResult(-1, "Donation not found");
            }

            var accountIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("accountId");
            if (accountIdClaim == null)
            {
                return new GiveandtakeResult(-1, "Account ID not found in token.");
            }

            var accountId = int.TryParse(accountIdClaim.Value, out var id) ? id : (int?)null;

            if (donation.Status == "Approved")
            {
                donation.Status = "Cancel";
                donation.ApprovedBy = accountId;
            }
            else if (donation.Status == "Cancel")
            {
                donation.Status = "Approved";
                donation.ApprovedBy = accountId;
            }
            else
            {
                return new GiveandtakeResult(-1, "Donation is in an invalid status.");
            }
            donation.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Donation>().UpdateAsync(donation);
            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult(1, "Donation status toggled successfully");
        }

        public async Task<IGiveandtakeResult> ToggleType(int donationId)
        {
            var donation = await _unitOfWork.GetRepository<Donation>()
                .SingleOrDefaultAsync(predicate: d => d.DonationId == donationId);

            if (donation == null)
            {
                return new GiveandtakeResult(-1, "Donation not found");
            }

            if (donation.Type == 1)
            {
                donation.Type = 2;
            }
            else if (donation.Type == 2)
            {
                donation.Type = 1;
            }
            else
            {
                return new GiveandtakeResult(-1, "Donation type is invalid. Only type 1 and 2 are allowed.");
            }

            donation.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Donation>().UpdateAsync(donation);
            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult(1, "Donation type toggled successfully");
        }

        public async Task<IGiveandtakeResult> ToggleType2(int donationId)
        {
            var donation = await _unitOfWork.GetRepository<Donation>()
                .SingleOrDefaultAsync(predicate: d => d.DonationId == donationId);

            if (donation == null)
            {
                return new GiveandtakeResult(-1, "Donation not found");
            }

            if (donation.Type == 1)
            {
                donation.Type = 3; 
            }
            else if (donation.Type == 3)
            {
                donation.Type = 1; 
            }
            else
            {
                return new GiveandtakeResult(-1, "Donation type is invalid. Only type 1 and 3 are allowed.");
            }

            donation.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Donation>().UpdateAsync(donation);
            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult(1, "Donation type toggled from 1 to 3 successfully.");
        }

        public async Task<IGiveandtakeResult> ToggleType3(int donationId)
        {
            var donation = await _unitOfWork.GetRepository<Donation>()
                .SingleOrDefaultAsync(predicate: d => d.DonationId == donationId);

            if (donation == null)
            {
                return new GiveandtakeResult(-1, "Donation not found");
            }

            if (donation.Type == 2)
            {
                donation.Type = 3; 
            }
            else if (donation.Type == 3)
            {
                donation.Type = 2; 
            }
            else
            {
                return new GiveandtakeResult(-1, "Donation type is invalid. Only type 2 and 3 are allowed.");
            }

            donation.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Donation>().UpdateAsync(donation);
            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult(1, "Donation type toggled from 2 to 3 successfully.");
        }

        public async Task<IGiveandtakeResult> CheckAndUpdateAllBannedAccountsDonations()
        {
            var accountRepository = _unitOfWork.GetRepository<Account>();
            var donationRepository = _unitOfWork.GetRepository<Donation>();

            var bannedAccounts = await accountRepository.GetListAsync(
                predicate: a => a.IsActive == false
            );

            int totalUpdatedDonations = 0;

            foreach (var account in bannedAccounts)
            {
                var donations = await donationRepository.GetListAsync(
                    predicate: d => d.AccountId == account.AccountId && d.Status != "Hiding"
                );

                foreach (var donation in donations)
                {
                    donation.Status = "Hiding";
                    donation.UpdatedAt = DateTime.Now;
                    donationRepository.UpdateAsync(donation);
                }

                totalUpdatedDonations += donations.Count;
            }

            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult(1, $"Updated {totalUpdatedDonations} donations to Hiding status for {bannedAccounts.Count} banned accounts");
        }

        public async Task<IGiveandtakeResult> CheckAndUpdateDonationsForActivatedAccounts()
        {
            var donationRepository = _unitOfWork.GetRepository<Donation>();
            var accountRepository = _unitOfWork.GetRepository<Account>();

            var activeAccountIds = await accountRepository.GetListAsync(
                predicate: a => a.IsActive == true,
                selector: a => a.AccountId.ToString()
            );

            var hidingDonations = await donationRepository.GetListAsync(
                predicate: d => d.Status == "Hiding" && activeAccountIds.Contains(d.AccountId.ToString())
            );

            if (!hidingDonations.Any())
            {
                Console.WriteLine("No hiding donations found for active accounts.");
                return new GiveandtakeResult(0, "No hiding donations to update.");
            }

            int totalUpdatedDonations = 0;

            foreach (var donation in hidingDonations)
            {
                donation.Status = "Approved";
                donation.UpdatedAt = DateTime.Now;
                Console.WriteLine($"Updating donation ID: {donation.DonationId} to Approved.");
                totalUpdatedDonations++;
                donationRepository.UpdateAsync(donation);
            }

            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult(1, $"Updated {totalUpdatedDonations} donations from Hiding to Approved status for activated accounts");
        }

        public async Task<IGiveandtakeResult> SearchDonations(string searchTerm, int page = 1, int pageSize = 8)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new GiveandtakeResult(-1, "Search term cannot be empty");
            }

            var repository = _unitOfWork.GetRepository<Donation>();
            var accountRepository = _unitOfWork.GetRepository<Account>();

            var allAccounts = await accountRepository.GetAllAsync();
            var accountDict = allAccounts.ToDictionary(a => a.AccountId, a => a.FullName);

            var searchResults = await repository.GetListAsync(
                predicate: d => d.Account.FullName.Contains(searchTerm) ||
                                d.Name.Contains(searchTerm) ||
                                d.Description.Contains(searchTerm),
                selector: d => new DonationDTO
                {
                    DonationId = d.DonationId,
                    AccountId = d.AccountId,
                    AccountName = d.Account.FullName,
                    CategoryId = d.CategoryId,
                    CategoryName = d.Category.CategoryName,
                    Name = d.Name,
                    Description = d.Description,
                    Point = d.Point,
                    Type = d.Type,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,
                    ApprovedBy = d.ApprovedBy,
                    ApprovedByName = d.ApprovedBy.HasValue && accountDict.ContainsKey(d.ApprovedBy.Value)
                        ? accountDict[d.ApprovedBy.Value]
                        : null,
                    TotalRating = d.TotalRating,
                    Status = d.Status,
                    DonationImages = d.DonationImages.Select(di => di.Url).ToList()
                },
                include: source => source
                    .Include(d => d.Account)
                    .Include(d => d.Category)
            );

            int totalItems = searchResults.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (page > totalPages) page = totalPages;

            if (totalItems == 0)
            {
                return new GiveandtakeResult(new PaginatedResult<DonationDTO>
                {
                    Items = new List<DonationDTO>(),
                    TotalItems = totalItems,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = totalPages
                });
            }

            var paginatedResults = searchResults
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paginatedResult = new PaginatedResult<DonationDTO>
            {
                Items = paginatedResults,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return new GiveandtakeResult(paginatedResult);
        }

        public async Task<IGiveandtakeResult> ChangeDonationStatus(int donationId, string newStatus)
        {
            var validStatuses = new[] { "Pending", "Approved", "Cancel", "Hiding", "Claimed" };

            if (!validStatuses.Contains(newStatus))
            {
                return new GiveandtakeResult(-1, "Invalid status provided.");
            }

            var donationRepository = _unitOfWork.GetRepository<Donation>();
            var donation = await donationRepository.SingleOrDefaultAsync(predicate: d => d.DonationId == donationId);

            if (donation == null)
            {
                return new GiveandtakeResult(-1, "Donation not found.");
            }

            donation.Status = newStatus;
            donation.UpdatedAt = DateTime.Now;

            donationRepository.UpdateAsync(donation);
            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult(1, $"Donation status changed to {newStatus} successfully.");
        }

        // Phần của Nguyên
        public async Task<IGiveandtakeResult> GetApprovedDonationByAccountAndType(int accountId)
        {
            var donationRepository = _unitOfWork.GetRepository<Donation>();
            var donations = await donationRepository.GetListAsync(
                predicate: d => d.AccountId == accountId && (d.Type == 1 || d.Type == 3) && d.Status == "Approved",
                selector: d => new DonationDTO
                {
                    DonationId = d.DonationId,
                    AccountId = d.AccountId,
                    AccountName = d.Account.FullName,
                    CategoryId = d.CategoryId,
                    CategoryName = d.Category.CategoryName,
                    Name = d.Name,
                    Description = d.Description,
                    Point = d.Point,
                    Type = d.Type,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,
                    ApprovedBy = d.ApprovedBy,
                    ApprovedByName = d.ApprovedBy.HasValue ? d.ApprovedBy.ToString() : null,
                    TotalRating = d.TotalRating,
                    Status = d.Status,
                    DonationImages = d.DonationImages.Select(di => di.Url).ToList(),
                    Feedbacks = d.Feedbacks.Select(f => new FeedbackDTO
                    {
                        FeedbackId = f.FeedbackId,
                        SenderId = f.SenderId,
                        SenderName = f.SenderId.HasValue ? f.SenderId.ToString() : null,
                        AccountId = f.AccountId,
                        AccountName = f.Account.FullName,
                        DonationId = d.DonationId,
                        DonationName = d.Name,
                        Rating = f.Rating,
                        Content = f.Content,
                        CreatedDate = f.CreatedDate,
                        FeedbackMediaUrls = f.FeedbackMedia.Select(fm => fm.MediaUrl).ToList()
                    }).ToList()
                },
                include: source => source
                    .Include(d => d.Account)
                    .Include(d => d.Category)
                    .Include(d => d.Feedbacks)
            );

            var sortedDonations = donations.OrderByDescending(d => d.CreatedAt).ToList();

            return new GiveandtakeResult(sortedDonations);
        }


    }
}
