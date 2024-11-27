using GiveandTake_Repo.DTOs.Member;
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
    public class MemberShipBusiness
    {
        private UnitOfWork _unitOfWork;

        public MemberShipBusiness()
        {
            _unitOfWork = new UnitOfWork();
        }

        public async Task<IGiveandtakeResult> GetAllMemberships(int page = 1, int pageSize = 8)
        {
            var membershipRepository = _unitOfWork.GetRepository<Membership>();
            var accountRepository = _unitOfWork.GetRepository<Account>();

            var allAccounts = await accountRepository.GetAllAsync();
            var accountDict = allAccounts.ToDictionary(a => a.AccountId, a => a.FullName);

            var allMemberships = await membershipRepository.GetListAsync(
                predicate: m => true,
                selector: m => new MembershipDTO
                {
                    MembershipId = m.MembershipId,
                    AccountId = m.AccountId,
                    PurchaseDate = m.PurchaseDate,
                    PremiumUntil = m.PremiumUntil,
                    Status = m.Status,
                    Amount = m.Amount,
                    FullName = m.Account.FullName
                },
                include: source => source
                    .Include(m => m.Account)
            );

            int totalItems = allMemberships.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (page > totalPages) page = totalPages;

            if (totalItems == 0)
            {
                return new GiveandtakeResult(new PaginatedResult<MembershipDTO>
                {
                    Items = new List<MembershipDTO>(),
                    TotalItems = totalItems,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = totalPages
                });
            }

            var paginatedMemberships = allMemberships
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paginatedResult = new PaginatedResult<MembershipDTO>
            {
                Items = paginatedMemberships,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return new GiveandtakeResult(paginatedResult);
        }

        public async Task<IGiveandtakeResult> GetMembershipByAccountId(int accountId)
        {
            var membershipRepository = _unitOfWork.GetRepository<Membership>();
            var memberships = await membershipRepository.GetListAsync(
                predicate: m => m.AccountId == accountId,
                selector: m => new MembershipDTO
                {
                    MembershipId = m.MembershipId,
                    AccountId = m.AccountId,
                    PurchaseDate = m.PurchaseDate,
                    PremiumUntil = m.PremiumUntil,
                    Status = m.Status,
                    Amount = m.Amount,
                    FullName = m.Account.FullName
                },
                include: source => source
                    .Include(m => m.Account)
            );

            if (!memberships.Any())
            {
                return new GiveandtakeResult(-1, "Memberships not found for the given account");
            }

            return new GiveandtakeResult(memberships);
        }

        public async Task<IGiveandtakeResult> CreateMembership(CreateMembershipDTO membershipInfo)
        {
            var account = await _unitOfWork.GetRepository<Account>()
                .FirstOrDefaultAsync(a => a.AccountId == membershipInfo.AccountId);
            if (account == null)
            {
                return new GiveandtakeResult(-1, "Account not found");
            }

            var newMembership = new Membership
            {
                AccountId = membershipInfo.AccountId,
                PurchaseDate = DateTime.UtcNow,
                PremiumUntil = DateTime.UtcNow.AddMonths(1),
                Status = membershipInfo.Status,
                Amount = membershipInfo.Amount
            };

            await _unitOfWork.GetRepository<Membership>().InsertAsync(newMembership);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccessful)
            {
                return new GiveandtakeResult(-1, "Create membership unsuccessfully");
            }

            return new GiveandtakeResult(1, "Membership created successfully");
        }

        public async Task<IGiveandtakeResult> CreateMembership3Months(CreateMembershipDTO membershipInfo)
        {
            var account = await _unitOfWork.GetRepository<Account>()
                .FirstOrDefaultAsync(a => a.AccountId == membershipInfo.AccountId);
            if (account == null)
            {
                return new GiveandtakeResult(-1, "Account not found");
            }

            var newMembership = new Membership
            {
                AccountId = membershipInfo.AccountId,
                PurchaseDate = DateTime.UtcNow,
                PremiumUntil = DateTime.UtcNow.AddMonths(3),
                Status = membershipInfo.Status,
                Amount = membershipInfo.Amount
            };

            await _unitOfWork.GetRepository<Membership>().InsertAsync(newMembership);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccessful)
            {
                return new GiveandtakeResult(-1, "Create membership unsuccessfully");
            }

            return new GiveandtakeResult(1, "Membership created successfully");
        }

        public async Task<IGiveandtakeResult> CreateMembership6Months(CreateMembershipDTO membershipInfo)
        {
            var account = await _unitOfWork.GetRepository<Account>()
                .FirstOrDefaultAsync(a => a.AccountId == membershipInfo.AccountId);
            if (account == null)
            {
                return new GiveandtakeResult(-1, "Account not found");
            }

            var newMembership = new Membership
            {
                AccountId = membershipInfo.AccountId,
                PurchaseDate = DateTime.UtcNow,
                PremiumUntil = DateTime.UtcNow.AddMonths(6),
                Status = membershipInfo.Status,
                Amount = membershipInfo.Amount
            };

            await _unitOfWork.GetRepository<Membership>().InsertAsync(newMembership);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccessful)
            {
                return new GiveandtakeResult(-1, "Create membership unsuccessfully");
            }

            return new GiveandtakeResult(1, "Membership created successfully");
        }

        public async Task<IGiveandtakeResult> UpdateMembership(int id, UpdateMembershipDTO membershipInfo)
        {
            var membershipRepository = _unitOfWork.GetRepository<Membership>();
            var existingMembership = await membershipRepository.SingleOrDefaultAsync(
                predicate: m => m.MembershipId == id
            );

            if (existingMembership == null)
            {
                return new GiveandtakeResult(-1, "Membership not found");
            }

            if (membershipInfo.PurchaseDate.HasValue)
                existingMembership.PurchaseDate = membershipInfo.PurchaseDate.Value;

            if (membershipInfo.PremiumUntil.HasValue)
                existingMembership.PremiumUntil = membershipInfo.PremiumUntil.Value;

            if (!string.IsNullOrEmpty(membershipInfo.Status))
                existingMembership.Status = membershipInfo.Status;

            if (!string.IsNullOrEmpty(membershipInfo.Amount))
                existingMembership.Amount = membershipInfo.Amount;

            membershipRepository.UpdateAsync(existingMembership);
            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult(1, "Membership updated successfully");
        }

        public async Task<IGiveandtakeResult> DeleteMembership(int id)
        {
            var membershipRepository = _unitOfWork.GetRepository<Membership>();

            var existingMembership = await membershipRepository.SingleOrDefaultAsync(
                predicate: m => m.MembershipId == id
            );

            if (existingMembership == null)
            {
                return new GiveandtakeResult(-1, "Membership not found");
            }

            membershipRepository.DeleteAsync(existingMembership);
            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult(1, "Membership deleted successfully");
        }

        public async Task UpdateMembershipAsync(MembershipDTO updateDto)
        {
            var membership = await _unitOfWork.GetRepository<Membership>()
                 .SingleOrDefaultAsync(m => m.AccountId == updateDto.AccountId, null, null);

            if (membership == null)
            {
                membership = new Membership
                {
                    AccountId = updateDto.AccountId,
                    PurchaseDate = updateDto.PurchaseDate,
                    PremiumUntil = updateDto.PremiumUntil,
                    Status = updateDto.Status
                };

                await _unitOfWork.GetRepository<Membership>().InsertAsync(membership);
            }
            else
            {
                membership.PurchaseDate = updateDto.PurchaseDate;
                membership.PremiumUntil = updateDto.PremiumUntil;
                membership.Status = updateDto.Status;

                _unitOfWork.GetRepository<Membership>().UpdateAsync(membership);
            }

            await _unitOfWork.CommitAsync();
        }

        public async Task<IGiveandtakeResult> CheckMembershipExpiry(int accountId)
        {
            var membershipRepository = _unitOfWork.GetRepository<Membership>();

            var membership = await membershipRepository.SingleOrDefaultAsync(
                predicate: m => m.AccountId == accountId
            );

            if (membership == null)
            {
                return new GiveandtakeResult(-1, "Không tìm thấy thông tin thành viên.");
            }

            if (!membership.PremiumUntil.HasValue)
            {
                return new GiveandtakeResult(-1, "Thành viên chưa có thời gian hết hạn Premium.");
            }

            var currentDate = DateTime.UtcNow;
            var premiumUntil = membership.PremiumUntil.Value;
            var remainingDays = (premiumUntil - currentDate).TotalDays;

            if (remainingDays <= 0)
            {
                return new GiveandtakeResult(-1, "Thời gian Premium đã hết hạn. Vui lòng gia hạn ngay để tiếp tục trải nghiệm.");
            }
            else if (remainingDays <= 3)
            {
                int daysRemaining = (int)Math.Ceiling(remainingDays);
                return new GiveandtakeResult(1, $"Còn {daysRemaining} ngày nữa là hết hạn Premium. Hãy gia hạn để tiếp tục duy trì trải nghiệm.");
            }
            else
            {
                return new GiveandtakeResult(1, "Thời hạn Premium của bạn vẫn còn dài. Hãy tận hưởng trải nghiệm.");
            }
        }

    }
}
