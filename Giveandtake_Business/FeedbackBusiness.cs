using GiveandTake_Repo.DTOs.Feedback;
using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Business
{
    public class FeedbackBusiness
    {
        private readonly UnitOfWork _unitOfWork;

        public FeedbackBusiness()
        {
            _unitOfWork = new UnitOfWork();
        }

        public async Task<IGiveandtakeResult> GetAllFeedbacks(int page = 1, int pageSize = 8)
        {
            var feedbackRepository = _unitOfWork.GetRepository<Feedback>();
            var accountRepository = _unitOfWork.GetRepository<Account>();
            var donationRepository = _unitOfWork.GetRepository<Donation>();

            var allAccounts = await accountRepository.GetAllAsync();
            var accountDict = allAccounts.ToDictionary(a => a.AccountId, a => a.FullName);

            var allDonations = await donationRepository.GetAllAsync();
            var donationDict = allDonations.ToDictionary(d => d.DonationId, d => d.AccountId);

            var allFeedbacks = await feedbackRepository.GetListAsync(
                predicate: f => true,
                selector: f => new FeedbackDTO
                {
                    FeedbackId = f.FeedbackId,
                    SenderId = f.SenderId,
                    SenderName = f.SenderId.HasValue && accountDict.ContainsKey(f.SenderId.Value)
                        ? accountDict[f.SenderId.Value]
                        : null,

                    AccountId = f.DonationId.HasValue && donationDict.ContainsKey(f.DonationId.Value)
                        ? donationDict[f.DonationId.Value]: null,

                    AccountName = f.DonationId != null && donationDict.ContainsKey(f.DonationId.Value)
                                  && donationDict[f.DonationId.Value].HasValue && accountDict.ContainsKey(donationDict[f.DonationId.Value].Value)
                                  ? accountDict[donationDict[f.DonationId.Value].Value]: null,

                    DonationId = f.DonationId,
                    DonationName = f.Donation != null ? f.Donation.Name : null,
                    Rating = f.Rating,
                    Content = f.Content,
                    CreatedDate = f.CreatedDate,
                    FeedbackMediaUrls = f.FeedbackMedia.Select(m => m.MediaUrl).ToList()
                },
                include: source => source
                    .Include(f => f.Account) 
                    .Include(f => f.Donation)
                    .Include(f => f.FeedbackMedia) 
            );

            int totalItems = allFeedbacks.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (page > totalPages) page = totalPages;

            if (totalItems == 0)
            {
                return new GiveandtakeResult(new PaginatedResult<FeedbackDTO>
                {
                    Items = new List<FeedbackDTO>(),
                    TotalItems = totalItems,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = totalPages
                });
            }

            var paginatedFeedbacks = allFeedbacks
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paginatedResult = new PaginatedResult<FeedbackDTO>
            {
                Items = paginatedFeedbacks,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return new GiveandtakeResult(paginatedResult);
        }
        public async Task<IGiveandtakeResult> GetFeedbackById(int feedbackId)
        {
            var feedbackRepository = _unitOfWork.GetRepository<Feedback>();
            var accountRepository = _unitOfWork.GetRepository<Account>();
            var feedback = await feedbackRepository.SingleOrDefaultAsync(
                predicate: f => f.FeedbackId == feedbackId,
                include: source => source
                    .Include(f => f.Donation)
                    .ThenInclude(d => d.Account) 
                    .Include(f => f.FeedbackMedia)
            );

            if (feedback == null)
            {
                return new GiveandtakeResult(-1, "Feedback not found");
            }
            string? senderName = null;
            if (feedback.SenderId.HasValue)
            {
                var senderAccount = await accountRepository.FirstOrDefaultAsync(a => a.AccountId == feedback.SenderId.Value);
                senderName = senderAccount?.FullName;
            }
            var feedbackDTO = new FeedbackDTO
            {
                FeedbackId = feedback.FeedbackId,
                SenderId = feedback.SenderId,
                SenderName = senderName,
                AccountId = feedback.Donation?.AccountId,
                AccountName = feedback.Donation?.Account?.FullName,
                DonationId = feedback.DonationId,
                DonationName = feedback.Donation?.Name,
                Rating = feedback.Rating,
                Content = feedback.Content,
                CreatedDate = feedback.CreatedDate,
                FeedbackMediaUrls = feedback.FeedbackMedia?.Select(m => m.MediaUrl).ToList() ?? new List<string>()
            };

            return new GiveandtakeResult(feedbackDTO);
        }


        public async Task<IGiveandtakeResult> CreateFeedback(CreateFeedbackDTO createFeedbackDto)
        {
            var senderAccount = await _unitOfWork.GetRepository<Account>()
                .FirstOrDefaultAsync(a => a.AccountId == createFeedbackDto.SenderId);
            if (senderAccount == null)
            {
                return new GiveandtakeResult(-1, "Sender account not found");
            }

            var donation = await _unitOfWork.GetRepository<Donation>()
                .FirstOrDefaultAsync(d => d.DonationId == createFeedbackDto.DonationId);
            if (donation == null)
            {
                return new GiveandtakeResult(-1, "Donation not found");
            }

            var existingFeedbacks = await _unitOfWork.GetRepository<Feedback>()
                .GetListAsync(predicate: f => f.DonationId == createFeedbackDto.DonationId && f.SenderId == createFeedbackDto.SenderId);

            if (existingFeedbacks.Any())
            {
                return new GiveandtakeResult(-1, "Feedback already exists for this donation by the sender");
            }

            var newFeedback = new Feedback
            {
                SenderId = createFeedbackDto.SenderId,
                DonationId = createFeedbackDto.DonationId,
                AccountId = donation.AccountId,
                Rating = createFeedbackDto.Rating,
                Content = createFeedbackDto.Content,
                CreatedDate = createFeedbackDto.CreateTime ?? DateTime.Now,
                FeedbackMedia = createFeedbackDto.FeedbackMediaUrls?.Select(url => new FeedbackMedium
                {
                    MediaUrl = url
                }).ToList() ?? new List<FeedbackMedium>()
            };

            await _unitOfWork.GetRepository<Feedback>().InsertAsync(newFeedback);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccessful)
            {
                return new GiveandtakeResult(-1, "Create feedback unsuccessfully");
            }

            if (donation.AccountId.HasValue)
            {
                await UpdateAccountRating(donation.AccountId.Value);
            }

            senderAccount.Point += senderAccount.IsPremium == true ? 10 : 5;
            _unitOfWork.GetRepository<Account>().UpdateAsync(senderAccount);
            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult(1, "Feedback created successfully");
        }

        public async Task<IGiveandtakeResult> CreateFeedbackWithoutPoints(CreateFeedbackDTO createFeedbackDto)
        {
            var senderAccount = await _unitOfWork.GetRepository<Account>()
                .FirstOrDefaultAsync(a => a.AccountId == createFeedbackDto.SenderId);
            if (senderAccount == null)
            {
                return new GiveandtakeResult(-1, "Sender account not found");
            }

            var donation = await _unitOfWork.GetRepository<Donation>()
                .FirstOrDefaultAsync(d => d.DonationId == createFeedbackDto.DonationId);
            if (donation == null)
            {
                return new GiveandtakeResult(-1, "Donation not found");
            }

            var existingFeedbacks = await _unitOfWork.GetRepository<Feedback>()
                .GetListAsync(predicate: f => f.DonationId == createFeedbackDto.DonationId && f.SenderId == createFeedbackDto.SenderId);

            if (existingFeedbacks.Any())
            {
                return new GiveandtakeResult(-1, "Feedback already exists for this donation by the sender");
            }

            // Create new feedback
            var newFeedback = new Feedback
            {
                SenderId = createFeedbackDto.SenderId,
                DonationId = createFeedbackDto.DonationId,
                AccountId = donation.AccountId,
                Rating = createFeedbackDto.Rating,
                Content = createFeedbackDto.Content,
                CreatedDate = createFeedbackDto.CreateTime ?? DateTime.Now,
                FeedbackMedia = createFeedbackDto.FeedbackMediaUrls?.Select(url => new FeedbackMedium
                {
                    MediaUrl = url
                }).ToList() ?? new List<FeedbackMedium>()
            };

            await _unitOfWork.GetRepository<Feedback>().InsertAsync(newFeedback);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccessful)
            {
                return new GiveandtakeResult(-1, "Create feedback unsuccessfully");
            }

            if (donation.AccountId.HasValue)
            {
                await UpdateAccountRating(donation.AccountId.Value);
            }

            return new GiveandtakeResult(1, "Feedback created successfully");
        }


        private async Task UpdateAccountRating(int accountId)
        {
            // Thay đổi cách gọi GetListAsync
            var feedbacks = await _unitOfWork.GetRepository<Feedback>()
                .GetListAsync(f => f.AccountId == accountId, null, null);

            if (feedbacks.Any())
            {
                // Tính toán rating trung bình
                var averageRating = feedbacks.Average(f => f.Rating);

                // Cập nhật rating vào bảng Account
                var accountRepository = _unitOfWork.GetRepository<Account>();

                // Sử dụng phương thức tương ứng để lấy account
                var account = await accountRepository.FirstOrDefaultAsync(a => a.AccountId == accountId);
                if (account != null)
                {
                    account.Rating = averageRating; // Cập nhật rating
                    accountRepository.UpdateAsync(account);
                    await _unitOfWork.CommitAsync();
                }
            }
        }


        public async Task<IGiveandtakeResult> UpdateFeedback(int id, UpdateFeedbackDTO feedbackInfo)
        {
            var feedbackRepository = _unitOfWork.GetRepository<Feedback>();
            var existingFeedback = await feedbackRepository.SingleOrDefaultAsync(
                predicate: f => f.FeedbackId == id
            );

            if (existingFeedback == null)
            {
                return new GiveandtakeResult(-1, "Feedback not found");
            }

            if (feedbackInfo.Rating.HasValue)
                existingFeedback.Rating = feedbackInfo.Rating.Value;

            if (!string.IsNullOrEmpty(feedbackInfo.Content))
                existingFeedback.Content = feedbackInfo.Content;

            if (feedbackInfo.CreateTime.HasValue)
                existingFeedback.CreatedDate = feedbackInfo.CreateTime.Value;

            if (feedbackInfo.FeedbackMediaUrls != null && feedbackInfo.FeedbackMediaUrls.Any())
            {
                await UpdateFeedbackMedia(existingFeedback.FeedbackId, feedbackInfo.FeedbackMediaUrls);
            }

            feedbackRepository.UpdateAsync(existingFeedback);
            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult(1, "Feedback updated successfully");
        }


        private async Task UpdateFeedbackMedia(int feedbackId, IEnumerable<string> feedbackMediaUrls)
        {
            var feedbackMediaRepository = _unitOfWork.GetRepository<FeedbackMedium>();

            var existingMedia = await feedbackMediaRepository.GetListAsync(
                predicate: fm => fm.FeedbackId == feedbackId
            );

            if (existingMedia != null && existingMedia.Any())
            {
                foreach (var media in existingMedia)
                {
                    feedbackMediaRepository.DeleteAsync(media);
                }
            }
            foreach (var mediaUrl in feedbackMediaUrls)
            {
                var newFeedbackMedia = new FeedbackMedium
                {
                    FeedbackId = feedbackId,
                    MediaUrl = mediaUrl
                };
                await feedbackMediaRepository.InsertAsync(newFeedbackMedia);
            }
            await _unitOfWork.CommitAsync();
        }


        public async Task<IGiveandtakeResult> DeleteFeedback(int id)
        {
            var feedbackRepository = _unitOfWork.GetRepository<Feedback>();
            var feedbackMediaRepository = _unitOfWork.GetRepository<FeedbackMedium>();

            var existingFeedback = await feedbackRepository.SingleOrDefaultAsync(
                predicate: f => f.FeedbackId == id
            );

            if (existingFeedback == null)
            {
                return new GiveandtakeResult(-1, "Feedback not found");
            }

            var existingMedia = await feedbackMediaRepository.GetListAsync(
                predicate: fm => fm.FeedbackId == id
            );

            if (existingMedia != null && existingMedia.Any())
            {
                foreach (var media in existingMedia)
                {
                    feedbackMediaRepository.DeleteAsync(media);
                }
            }

            feedbackRepository.DeleteAsync(existingFeedback);
            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult(1, "Feedback deleted successfully");
        }


        public async Task<IGiveandtakeResult> GetFeedbacksBySenderId(int senderId, int page = 1, int pageSize = 8)
        {
            var feedbackRepository = _unitOfWork.GetRepository<Feedback>();
            var accountRepository = _unitOfWork.GetRepository<Account>();
            var donationRepository = _unitOfWork.GetRepository<Donation>();

            var allAccounts = await accountRepository.GetAllAsync();
            var accountDict = allAccounts.ToDictionary(a => a.AccountId, a => a.FullName);

            var allDonations = await donationRepository.GetAllAsync();
            var donationDict = allDonations.ToDictionary(d => d.DonationId, d => d.AccountId);

            var feedbacksBySenderId = await feedbackRepository.GetListAsync(
                predicate: f => f.SenderId == senderId,
                selector: f => new FeedbackDTO
                {
                    FeedbackId = f.FeedbackId,
                    SenderId = f.SenderId,
                    SenderName = f.SenderId.HasValue && accountDict.ContainsKey(f.SenderId.Value)
                        ? accountDict[f.SenderId.Value]
                        : null,
                    AccountId = f.DonationId.HasValue && donationDict.ContainsKey(f.DonationId.Value)
                        ? donationDict[f.DonationId.Value]
                        : null,
                    AccountName = f.DonationId != null && donationDict.ContainsKey(f.DonationId.Value)
                                  && donationDict[f.DonationId.Value].HasValue
                                  && accountDict.ContainsKey(donationDict[f.DonationId.Value].Value)
                                  ? accountDict[donationDict[f.DonationId.Value].Value]
                                  : null,
                    DonationId = f.DonationId,
                    DonationName = f.Donation != null ? f.Donation.Name : null,
                    Rating = f.Rating,
                    Content = f.Content,
                    CreatedDate = f.CreatedDate,
                    FeedbackMediaUrls = f.FeedbackMedia.Select(m => m.MediaUrl).ToList()
                },
                include: source => source
                    .Include(f => f.Account)
                    .Include(f => f.Donation)
                    .Include(f => f.FeedbackMedia)
            );

            int totalItems = feedbacksBySenderId.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (page > totalPages) page = totalPages;

            if (totalItems == 0)
            {
                return new GiveandtakeResult(new PaginatedResult<FeedbackDTO>
                {
                    Items = new List<FeedbackDTO>(),
                    TotalItems = totalItems,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = totalPages
                });
            }

            var paginatedFeedbacks = feedbacksBySenderId
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paginatedResult = new PaginatedResult<FeedbackDTO>
            {
                Items = paginatedFeedbacks,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return new GiveandtakeResult(paginatedResult);
        }


        public async Task<IGiveandtakeResult> GetFeedbacksByAccountId(int accountId, int page = 1, int pageSize = 8)
        {
            var feedbackRepository = _unitOfWork.GetRepository<Feedback>();
            var accountRepository = _unitOfWork.GetRepository<Account>();
            var donationRepository = _unitOfWork.GetRepository<Donation>();

            var allAccounts = await accountRepository.GetAllAsync();
            var accountDict = allAccounts.ToDictionary(a => a.AccountId, a => a.FullName);

            var allDonations = await donationRepository.GetAllAsync();
            var donationDict = allDonations.ToDictionary(d => d.DonationId, d => d.AccountId);

            var feedbacksByAccountId = await feedbackRepository.GetListAsync(
                predicate: f => f.Donation.AccountId == accountId,
                selector: f => new FeedbackDTO
                {
                    FeedbackId = f.FeedbackId,
                    SenderId = f.SenderId,
                    SenderName = f.SenderId.HasValue && accountDict.ContainsKey(f.SenderId.Value)
                        ? accountDict[f.SenderId.Value]
                        : null,
                    AccountId = f.DonationId.HasValue && donationDict.ContainsKey(f.DonationId.Value)
                        ? donationDict[f.DonationId.Value]
                        : null,
                    AccountName = f.DonationId != null && donationDict.ContainsKey(f.DonationId.Value)
                                  && donationDict[f.DonationId.Value].HasValue
                                  && accountDict.ContainsKey(donationDict[f.DonationId.Value].Value)
                                  ? accountDict[donationDict[f.DonationId.Value].Value]
                                  : null,
                    DonationId = f.DonationId,
                    DonationName = f.Donation != null ? f.Donation.Name : null,
                    Rating = f.Rating,
                    Content = f.Content,
                    CreatedDate = f.CreatedDate,
                    FeedbackMediaUrls = f.FeedbackMedia.Select(m => m.MediaUrl).ToList()
                },
                include: source => source
                    .Include(f => f.Account)
                    .Include(f => f.Donation)
                    .Include(f => f.FeedbackMedia)
            );

            int totalItems = feedbacksByAccountId.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (page > totalPages) page = totalPages;

            if (totalItems == 0)
            {
                return new GiveandtakeResult(new PaginatedResult<FeedbackDTO>
                {
                    Items = new List<FeedbackDTO>(),
                    TotalItems = totalItems,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = totalPages
                });
            }

            var paginatedFeedbacks = feedbacksByAccountId
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paginatedResult = new PaginatedResult<FeedbackDTO>
            {
                Items = paginatedFeedbacks,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return new GiveandtakeResult(paginatedResult);
        }
    }
}