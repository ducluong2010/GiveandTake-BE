using GiveandTake_Repo.DTOs.Donation;
using GiveandTake_Repo.DTOs.Favorite;
using GiveandTake_Repo.DTOs.Feedback;
using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Business
{
    public class FavoriteBusiness
    {
        public readonly UnitOfWork _unitOfWork;

        public FavoriteBusiness()
        {
            _unitOfWork = new UnitOfWork();
        }

        public async Task<IGiveandtakeResult> GetAllFavorites()
        {
            var favoriteList = await _unitOfWork.GetRepository<Favorite>()
                .GetListAsync(selector: x => new GetFavoriteDTO
                {
                    FavoriteId = x.FavoriteId,
                    AccountId = x.AccountId,
                    CategoryId = x.CategoryId
                });
            return new GiveandtakeResult(favoriteList);
        }

        public async Task<IGiveandtakeResult> GetFavoriteById(int id)
        {
            var favorite = await _unitOfWork.GetRepository<Favorite>()
                .SingleOrDefaultAsync(predicate: c => c.FavoriteId == id,
                                      selector: x => new GetFavoriteDTO
                                      {
                                          FavoriteId = x.FavoriteId,
                                          AccountId = x.AccountId,
                                          CategoryId = x.CategoryId
                                      });

            return new GiveandtakeResult(favorite);
        }

        public async Task<IGiveandtakeResult> GetFavoritesByAccountId(int accountId)
        {
            var favorites = await _unitOfWork.GetRepository<Favorite>()
                .GetListAsync(predicate: c => c.AccountId == accountId,
                              selector: x => new GetFavoriteDTO
                              {
                                  FavoriteId = x.FavoriteId,
                                  AccountId = x.AccountId,
                                  CategoryId = x.CategoryId
                              });

            return new GiveandtakeResult(favorites);
        }

        public async Task<IGiveandtakeResult> AddFavorite(int accountId, FavoriteDTO favoriteDTO)
        {
            GiveandtakeResult result = new GiveandtakeResult();

            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate:
            c => c.AccountId == accountId);
            if (account == null)
            {
                return new GiveandtakeResult(-1, "Account not found");
            }

            // Check if the favorite category already exists for the account
            var existingFavorite = await _unitOfWork.GetRepository<Favorite>()
                .SingleOrDefaultAsync(predicate: f => f.AccountId == accountId && f.CategoryId == favoriteDTO.CategoryId);
            if (existingFavorite != null)
            {
                return new GiveandtakeResult(-1, "Favorite category already exists for this account");
            }

            Favorite newFavorite = new Favorite
            {
                AccountId = accountId,
                CategoryId = favoriteDTO.CategoryId
            };
            await _unitOfWork.GetRepository<Favorite>().InsertAsync(newFavorite);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (isSuccessful)
            {
                result = new GiveandtakeResult(1, "Add new favorite successfully");
            }
            else
            {
                result.Status = -1;
                result.Message = "Cannot add new favorite";
            }
            return result;
        }

        public async Task<IGiveandtakeResult> DeleteFavorite(int id)
        {
            var favorite = await _unitOfWork.GetRepository<Favorite>()
                .SingleOrDefaultAsync(predicate: c => c.FavoriteId == id);

            if (favorite == null)
            {
                return new GiveandtakeResult(-1, "Favorite not found");
            }

            _unitOfWork.GetRepository<Favorite>().DeleteAsync(favorite);
            await _unitOfWork.CommitAsync();
            return new GiveandtakeResult(1, "Remove favorite successfully");
        }

        public async Task<IGiveandtakeResult> GetFavoriteDonationsByCategory(int accountId)
        {
            // Kiểm tra tài khoản hợp lệ
            var account = await _unitOfWork.GetRepository<Account>()
                .SingleOrDefaultAsync(predicate: a => a.AccountId == accountId);
            if (account == null)
            {
                return new GiveandtakeResult(-1, "Account not found");
            }

            // Lấy danh sách category ID từ mục yêu thích của account
            var favoriteCategories = await _unitOfWork.GetRepository<Favorite>()
                .GetListAsync(predicate: f => f.AccountId == accountId);
            var favoriteCategoryIds = favoriteCategories.Select(f => f.CategoryId).ToList();

            // Kiểm tra nếu account không có danh mục yêu thích
            if (!favoriteCategoryIds.Any())
            {
                return new GiveandtakeResult(-1, "No favorite categories found for this account");
            }

            // Lấy các donation thuộc danh mục yêu thích với status "Approved"
            var donations = await _unitOfWork.GetRepository<Donation>()
                .GetListAsync(
                    predicate: d => favoriteCategoryIds.Contains(d.CategoryId) && d.Status == "Approved",
                    include: source => source
                        .Include(d => d.DonationImages)
                        .Include(d => d.Feedbacks)
                        .Include(d => d.Account)
                );

            var accountRepository = _unitOfWork.GetRepository<Account>();

            // Chuyển đổi sang DTO
            var donationDTOs = new List<DonationDTO>();
            foreach (var donation in donations)
            {
                // Lấy tên người phê duyệt (nếu có)
                string approverName = null;
                if (donation.ApprovedBy.HasValue)
                {
                    var approver = await accountRepository.SingleOrDefaultAsync(predicate: a => a.AccountId == donation.ApprovedBy.Value);
                    approverName = approver?.FullName;
                }

                string categoryName = null;
                if (donation.CategoryId.HasValue)
                {
                    var category = await _unitOfWork.GetRepository<Category>()
                        .SingleOrDefaultAsync(predicate: c => c.CategoryId == donation.CategoryId.Value);
                    categoryName = category?.CategoryName;
                }

                var donationDTO = new DonationDTO
                {
                    DonationId = donation.DonationId,
                    AccountId = donation.AccountId,
                    AccountName = donation.Account?.FullName,
                    CategoryId = donation.CategoryId,
                    CategoryName = categoryName,
                    Name = donation.Name,
                    Description = donation.Description,
                    Point = donation.Point,
                    CreatedAt = donation.CreatedAt,
                    UpdatedAt = donation.UpdatedAt,
                    ApprovedBy = donation.ApprovedBy,
                    ApprovedByName = approverName,
                    TotalRating = donation.TotalRating,
                    Status = donation.Status,
                    Type = donation.Type,
                    DonationImages = donation.DonationImages?.Select(di => di.Url).ToList() ?? new List<string>(),
                    Feedbacks = donation.Feedbacks.Select(f => new FeedbackDTO
                    {
                        FeedbackId = f.FeedbackId,
                        AccountId = f.AccountId,
                        AccountName = f.Account?.FullName,
                        DonationId = f.DonationId,
                        DonationName = f.Donation?.Name,
                        Rating = f.Rating,
                        Content = f.Content,
                        CreatedDate = f.CreatedDate,
                        FeedbackMediaUrls = f.FeedbackMedia.Select(fm => fm.MediaUrl).ToList()
                    }).ToList()
                };

                donationDTOs.Add(donationDTO);
            }
            return new GiveandtakeResult(donationDTOs);
        }

        public async Task<IGiveandtakeResult> AddMultipleFavorites(int accountId, List<FavoriteDTO> favoriteDTOs)
        {
            GiveandtakeResult result = new GiveandtakeResult();

            // Kiểm tra xem tài khoản có tồn tại không
            var account = await _unitOfWork.GetRepository<Account>()
                .SingleOrDefaultAsync(predicate: a => a.AccountId == accountId);
            if (account == null)
            {
                return new GiveandtakeResult(-1, "Account not found");
            }

            // Danh sách để lưu các mục yêu thích mới
            List<Favorite> newFavorites = new List<Favorite>();

            foreach (var favoriteDTO in favoriteDTOs)
            {
                // Kiểm tra xem danh mục yêu thích đã tồn tại chưa
                var existingFavorite = await _unitOfWork.GetRepository<Favorite>()
                    .SingleOrDefaultAsync(predicate: f => f.AccountId == accountId && f.CategoryId == favoriteDTO.CategoryId);

                if (existingFavorite == null)
                {
                    // Thêm mục yêu thích mới vào danh sách
                    newFavorites.Add(new Favorite
                    {
                        AccountId = accountId,
                        CategoryId = favoriteDTO.CategoryId
                    });
                }

                else if (existingFavorite != null)
                {
                    return new GiveandtakeResult(-1, "Favorite category already exists for this account");
                }
            }

            // Chèn các mục yêu thích mới
            await _unitOfWork.GetRepository<Favorite>().InsertRangeAsync(newFavorites);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (isSuccessful)
            {
                result = new GiveandtakeResult(1, $"{newFavorites.Count} favorites added successfully");
            }
            else
            {
                result.Status = -1;
                result.Message = "Failed to add new favorites";
            }

            return result;
        }

    }
}
