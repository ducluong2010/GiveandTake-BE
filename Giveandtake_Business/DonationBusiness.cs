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

        public async Task<IGiveandtakeResult> GetDonationById(int donationId)
        {
            var donationRepository = _unitOfWork.GetRepository<Donation>();
            var accountRepository = _unitOfWork.GetRepository<Account>();

            // Fetch the donation with related entities
            var donation = await donationRepository.SingleOrDefaultAsync(
                predicate: d => d.DonationId == donationId,
                include: source => source
                    .Include(d => d.Account)
                    .Include(d => d.Category)
                    .Include(d => d.DonationImages)
            );

            if (donation == null)
            {
                return new GiveandtakeResult(404, "Donation not found");
            }

            // Fetch the approver's name if ApprovedBy is not null
            string approverName = null;
            if (donation.ApprovedBy.HasValue)
            {
                var approver = await accountRepository.SingleOrDefaultAsync(
                    predicate: a => a.AccountId == donation.ApprovedBy.Value,
                    orderBy: null,
                    include: null
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
                CreatedAt = donation.CreatedAt,
                UpdatedAt = donation.UpdatedAt,
                ApprovedBy = donation.ApprovedBy,
                ApprovedByName = approverName,
                TotalRating = donation.TotalRating,
                Status = donation.Status,
                DonationImages = donation.DonationImages?.Select(di => di.Url).ToList() ?? new List<string>()
            };

            return new GiveandtakeResult(donationDTO);
        }

        //public async Task<IGiveandtakeResult> UpdateDonation(int id, CreateUpdateDonationDTO donationInfo)
        //{
        //    var category = await _unitOfWork.GetRepository<Category>()
        //        .FirstOrDefaultAsync(c => c.CategoryId == donationInfo.CategoryId);

        //    if (category == null)
        //    {
        //        return new GiveandtakeResult(-1, "Category not found");
        //    }

        //    var donation = await _unitOfWork.GetRepository<Donation>()
        //        .SingleOrDefaultAsync(predicate: d => d.DonationId == id);

        //    if (donation == null)
        //    {
        //        return new GiveandtakeResult(-1, "Donation not found");
        //    }

        //    donation.Name = !string.IsNullOrEmpty(donationInfo.Name) ? donationInfo.Name : donation.Name;
        //    donation.Description = !string.IsNullOrEmpty(donationInfo.Description) ? donationInfo.Description : donation.Description;
        //    donation.Point = category.Point;
        //    donation.ApprovedBy = donationInfo.ApprovedBy ?? donation.ApprovedBy;
        //    donation.TotalRating = donationInfo.TotalRating ?? donation.TotalRating;
        //    donation.Status = !string.IsNullOrEmpty(donationInfo.Status) ? donationInfo.Status : donation.Status;
        //    donation.UpdatedAt = DateTime.Now;

        //    _unitOfWork.GetRepository<Donation>().UpdateAsync(donation);
        //    await _unitOfWork.CommitAsync();
        //    return new GiveandtakeResult(1, "Donation updated successfully");
        //}
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
                Status = "Pending",
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
    }
}
