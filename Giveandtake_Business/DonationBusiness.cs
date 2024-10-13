using GiveandTake_Repo.DTOs.Account;
using GiveandTake_Repo.DTOs.Donation;
using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Giveandtake_Business
{
    public class DonationBusiness
    {
        private readonly UnitOfWork _unitOfWork;

        public DonationBusiness()
        {
            _unitOfWork = new UnitOfWork();
        }

        public async Task<IGiveandtakeResult> GetAllDonations(int page = 1, int pageSize = 8)
        {
            var repository = _unitOfWork.GetRepository<Donation>();

            // Get all active donations based on Status
            var allDonations = await repository.GetListAsync(
            predicate: d => true, 
            selector: d => new DonationDTO
            {
                DonationId = d.DonationId,
                AccountId = d.AccountId,
                CategoryId = d.CategoryId,
                Name = d.Name,
                Description = d.Description,
                Point = d.Point,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt,
                ApprovedBy = d.ApprovedBy,
                TotalRating = d.TotalRating,
                Status = d.Status,
                DonationImages = d.DonationImages.Select(di => di.Url).ToList()
            });


            // Đếm tổng số donations
            int totalItems = allDonations.Count;
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Điều chỉnh trang nếu nó vượt quá tổng số trang
            if (page > totalPages) page = totalPages;

            // Nếu không có donations nào, trả về danh sách trống
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

            // Thực hiện phân trang
            var paginatedDonations = allDonations
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Tạo kết quả phân trang
            var paginatedResult = new PaginatedResult<DonationDTO>
            {
                Items = paginatedDonations, // Danh sách donations đã phân trang
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return new GiveandtakeResult(paginatedResult);
        }

        public async Task<IGiveandtakeResult> GetDonationById(int donationId)
        {
            var donation = await _unitOfWork.GetRepository<Donation>()
                .SingleOrDefaultAsync(predicate: d => d.DonationId == donationId,
                                      selector: x => new DonationDTO
                                      {
                                          DonationId = x.DonationId,
                                          AccountId = x.AccountId,
                                          CategoryId = x.CategoryId,
                                          Name = x.Name,
                                          Description = x.Description,
                                          Point = x.Point,
                                          CreatedAt = x.CreatedAt,
                                          UpdatedAt = x.UpdatedAt,
                                          ApprovedBy = x.ApprovedBy,
                                          TotalRating = x.TotalRating,
                                          Status = x.Status,
                                          DonationImages = x.DonationImages.Select(di => di.Url).ToList()
                                      });
            return new GiveandtakeResult(donation);
        }

        public async Task<IGiveandtakeResult> UpdateDonation(int id, CreateUpdateDonationDTO donationInfo)
        {
            var category = await _unitOfWork.GetRepository<Category>()
                .FirstOrDefaultAsync(c => c.CategoryId == donationInfo.CategoryId);

            if (category == null)
            {
                return new GiveandtakeResult(-1, "Category not found");
            }

            var donation = await _unitOfWork.GetRepository<Donation>()
                .SingleOrDefaultAsync(predicate: d => d.DonationId == id);

            if (donation == null)
            {
                return new GiveandtakeResult(-1, "Donation not found");
            }

            donation.Name = !string.IsNullOrEmpty(donationInfo.Name) ? donationInfo.Name : donation.Name;
            donation.Description = !string.IsNullOrEmpty(donationInfo.Description) ? donationInfo.Description : donation.Description;
            donation.Point = category.Point;
            donation.ApprovedBy = donationInfo.ApprovedBy ?? donation.ApprovedBy;
            donation.TotalRating = donationInfo.TotalRating ?? donation.TotalRating;
            donation.Status = !string.IsNullOrEmpty(donationInfo.Status) ? donationInfo.Status : donation.Status;
            donation.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Donation>().UpdateAsync(donation);
            await _unitOfWork.CommitAsync();
            return new GiveandtakeResult(1, "Donation updated successfully");
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
                Status = "Pending",
                DonationImages = new List<DonationImage>() // Xử lý nếu cần thêm DonationImages
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
                .SingleOrDefaultAsync(predicate: d => d.DonationId == id);

            if (donation == null)
            {
                return new GiveandtakeResult(-1, "Donation not found");
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

            if (donation.Status == "Pending")
            {
                donation.Status = "Approved";
            }
            else if (donation.Status == "Approved")
            {
                donation.Status = "Pending";
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

            if (donation.Status == "Pending")
            {
                donation.Status = "Cancel";
            }
            else if (donation.Status == "Cancel")
            {
                donation.Status = "Pending";
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

            if (donation.Status == "Approved")
            {
                donation.Status = "Cancel";
            }
            else if (donation.Status == "Cancel")
            {
                donation.Status = "Approved";
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

    }
}
