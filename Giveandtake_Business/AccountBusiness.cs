using Giveandtake_Business.Utils;
using GiveandTake_Repo.DTOs.Account;
using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Business
{
    public class AccountBusiness
    {
        private readonly UnitOfWork _unitOfWork;

        public AccountBusiness()
        {
            _unitOfWork ??= new UnitOfWork();
        }

        #region Authentication

        // Login method
        public async Task<IGiveandtakeResult> Login(string email, string password)
        {
            var account = await _unitOfWork.GetRepository<Account>()
                .SingleOrDefaultAsync(predicate: a => a.Email.Equals(email) && a.Password.Equals(password));

            if (account == null || !(bool)account.IsActive)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = account == null ? "Incorrect Email or Password" : "Your account is currently inactive (banned)"
                };
            }

            return new GiveandtakeResult
            {
                Status = 1,
                Message = "Login successfully",
                Data = new
                {
                    AccountId = account.AccountId,
                    JwtToken = JwtUtils.GenerateJwtToken(account)
                }
            };
        }


        // Register method
        public async Task<IGiveandtakeResult> Register(UserRegisterDTO registerDto)
        {
            var repo = _unitOfWork.GetRepository<Account>();
            var existingAccountByEmail = await repo.SingleOrDefaultAsync(predicate: a => a.Email.Equals(registerDto.Email) && a.IsActive == true);
            var existingAccountByPhone = await repo.SingleOrDefaultAsync(predicate: a => a.Phone != null && a.Phone.Equals(registerDto.Phone));

            if (existingAccountByEmail != null)
                return new GiveandtakeResult { Status = -1, Message = "Email has already been used by an active account" };

            if (existingAccountByPhone != null)
                return new GiveandtakeResult { Status = -1, Message = "Phone number has already been used" };

            var newAccount = new Account
            {
                FullName = registerDto.FullName,
                Password = registerDto.Password,
                Email = registerDto.Email,
                Phone = registerDto.Phone,
                Address = registerDto.Address,
                Point = 0,
                AvatarUrl = registerDto.AvatarUrl,
                RoleId = 3,
                IsActive = false,
                IsPremium = false,
                PremiumUntil = null,
                ActiveTime = null
            };

            await repo.InsertAsync(newAccount);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            return new GiveandtakeResult
            {
                Status = isSuccessful ? 1 : -1,
                Message = isSuccessful ? "Register successfully" : "Register unsuccessfully",
                Data = isSuccessful ? JwtUtils.GenerateJwtToken(newAccount) : null
            };
        }
        #endregion

        #region Account

        // Method to get all active accounts
        public async Task<IGiveandtakeResult> GetAllAccount(int page = 1, int pageSize = 8)
        {
            var repo = _unitOfWork.GetRepository<Account>();

            var allAccounts = await repo.GetListAsync(
                predicate: a => (bool)a.IsActive,
                selector: a => new GetAccountDTO
                {
                    AccountId = a.AccountId,
                    Address = a.Address,
                    AvatarUrl = a.AvatarUrl,
                    IsActive = a.IsActive,
                    Email = a.Email,
                    FullName = a.FullName,
                    Password = a.Password,
                    Phone = a.Phone,
                    Point = a.Point,
                    RoleId = a.RoleId,
                    IsPremium = a.IsPremium,
                    PremiumUnti = a.PremiumUntil,
                    ChatId = a.AccountId,
                    MessageId = a.AccountId,
                    Rating = a.Rating,
                    ActiveTime = a.ActiveTime
                }
            );

            int totalItems = allAccounts.Count;
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            page = Math.Min(page, totalPages);

            var paginatedResult = new PaginatedResult<GetAccountDTO>
            {
                Items = allAccounts.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return new GiveandtakeResult(paginatedResult);
        }


        // Method to get account information by account ID
        public async Task<IGiveandtakeResult> GetAccountInfo(int accountId)
        {
            var acc = await _unitOfWork.GetRepository<Account>()
                .SingleOrDefaultAsync(
                    predicate: a => a.AccountId == accountId,
                    selector: a => new GetAccountDTO
                    {
                        AccountId = a.AccountId,
                        Address = a.Address,
                        AvatarUrl = a.AvatarUrl,
                        IsActive = a.IsActive,
                        Email = a.Email,
                        FullName = a.FullName,
                        Password = a.Password,
                        Phone = a.Phone,
                        Point = a.Point,
                        RoleId = a.RoleId,
                        IsPremium = a.IsPremium,
                        PremiumUnti = a.PremiumUntil,
                        ChatId = a.AccountId,
                        MessageId = a.AccountId,
                        Rating = a.Rating,
                        ActiveTime = a.ActiveTime
                    });

            return acc != null ? new GiveandtakeResult(acc) : new GiveandtakeResult();
        }


        // Method to get account information by email
        public async Task<IGiveandtakeResult> GetAccountInfoByEmail(string email)
        {
            var acc = await _unitOfWork.GetRepository<Account>()
                .SingleOrDefaultAsync(
                    predicate: a => a.Email.Equals(email),
                    selector: a => new GetAccountDTO
                    {
                        AccountId = a.AccountId,
                        Address = a.Address,
                        AvatarUrl = a.AvatarUrl,
                        IsActive = a.IsActive,
                        Email = a.Email,
                        FullName = a.FullName,
                        Password = a.Password,
                        Phone = a.Phone,
                        Point = a.Point,
                        RoleId = a.RoleId,
                        IsPremium = a.IsPremium,
                        PremiumUnti = a.PremiumUntil,
                        ChatId = a.AccountId,
                        MessageId = a.AccountId,
                        Rating = a.Rating,
                        ActiveTime = a.ActiveTime
                    });

            return acc != null ? new GiveandtakeResult(acc) : new GiveandtakeResult();
        }


        // Method to create a new account (used by admin)
        public async Task<IGiveandtakeResult> CreateAccount(AccountDTO inputedAccount)
        {
            var repo = _unitOfWork.GetRepository<Account>();

            var existingAccountByEmail = await repo.SingleOrDefaultAsync(predicate: a => a.Email.Equals(inputedAccount.Email));
            var existingAccountByPhone = await repo.SingleOrDefaultAsync(predicate: a => a.Phone != null && a.Phone.Equals(inputedAccount.Phone));
            var existingTimeExists = await repo.SingleOrDefaultAsync(predicate: a => a.ActiveTime == inputedAccount.ActiveTime);

            if (existingAccountByEmail != null)
                return new GiveandtakeResult { Status = -1, Message = "Email has already used" };

            if (existingAccountByPhone != null)
                return new GiveandtakeResult { Status = -1, Message = "Phone number has already been used" };

            if (existingTimeExists != null)
                return new GiveandtakeResult { Status = -1, Message = "Active time already assigned to another staff member" };

            var newAccount = new Account
            {
                FullName = inputedAccount.FullName,
                Password = inputedAccount.Password,
                Email = inputedAccount.Email,
                Phone = inputedAccount.Phone,
                Address = inputedAccount.Address,
                Point = inputedAccount.Point,
                AvatarUrl = inputedAccount.AvatarUrl,
                RoleId = inputedAccount.RoleId,
                IsActive = true,
                IsPremium = false,
                PremiumUntil = null,
                ActiveTime = inputedAccount.ActiveTime
            };

            await repo.InsertAsync(newAccount);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            return new GiveandtakeResult
            {
                Status = isSuccessful ? 1 : -1,
                Message = isSuccessful ? "Create Successful" : "Create unsuccessfully"
            };
        }


        // Method to update account information
        public async Task<IGiveandtakeResult> UpdateAccountInfo(int id, AccountDTO accInfo)
        {
            Account currentAcc = await _unitOfWork.GetRepository<Account>()
                .SingleOrDefaultAsync(predicate: a => a.AccountId == id);

            if (currentAcc == null)
            {
                return new GiveandtakeResult(-1, "Account cannot be found");
            }
            else
            {
                currentAcc.FullName = String.IsNullOrEmpty(accInfo.FullName) ? currentAcc.FullName : accInfo.FullName;
                currentAcc.RoleId = accInfo.RoleId == 0 ? currentAcc.RoleId : accInfo.RoleId;
                currentAcc.Email = string.IsNullOrEmpty(accInfo.Email) ? currentAcc.Email : accInfo.Email;
                currentAcc.Password = String.IsNullOrEmpty(accInfo.Password) ? currentAcc.Password : accInfo.Password;
                currentAcc.Address = String.IsNullOrEmpty(accInfo.Address) ? currentAcc.Address : accInfo.Address;
                currentAcc.Phone = String.IsNullOrEmpty(accInfo.Phone) ? currentAcc.Phone : accInfo.Phone;
                currentAcc.AvatarUrl = String.IsNullOrEmpty(accInfo.AvatarUrl) ? currentAcc.AvatarUrl : accInfo.AvatarUrl;
                currentAcc.Point = accInfo.Point;
                currentAcc.IsActive = accInfo.IsActive;
                currentAcc.IsPremium = accInfo.IsPremium;
                currentAcc.PremiumUntil = accInfo.PremiumUnti;
                currentAcc.ActiveTime = accInfo.ActiveTime;

                _unitOfWork.GetRepository<Account>().UpdateAsync(currentAcc);
                await _unitOfWork.CommitAsync();
            }

            return new GiveandtakeResult(accInfo);
        }


        // Method to delete an account
        public async Task<IGiveandtakeResult> DeleteAccount(int id)
        {
            Account currentAcc = await _unitOfWork.GetRepository<Account>()
                .SingleOrDefaultAsync(predicate: a => a.AccountId == id);

            if (currentAcc == null)
            {
                return new GiveandtakeResult(-1, "Account cannot be found");
            }
            else
            {
                _unitOfWork.GetRepository<Account>().DeleteAsync(currentAcc);
                await _unitOfWork.CommitAsync();
            }

            return new GiveandtakeResult("Delete Successful");
        }


        // Method to ban an account
        public async Task<IGiveandtakeResult> BanAccount(int accountId)
        {
            Account currentAcc = await _unitOfWork.GetRepository<Account>()
                .SingleOrDefaultAsync(predicate: a => a.AccountId == accountId);
            if (currentAcc == null)
            {
                return new GiveandtakeResult(-1, "Account cannot be found");
            }
            else
            {
                currentAcc.IsActive = false;

                _unitOfWork.GetRepository<Account>().UpdateAsync(currentAcc);
                await _unitOfWork.CommitAsync();
            }
            return new GiveandtakeResult("Banned");
        }

        // Method to unban an account
        public async Task<IGiveandtakeResult> UnbanAccount(int accountId)
        {
            Account currentAcc = await _unitOfWork.GetRepository<Account>()
                .SingleOrDefaultAsync(predicate: a => a.AccountId == accountId);
            if (currentAcc == null)
            {
                return new GiveandtakeResult(-1, "Account cannot be found");
            }
            else
            {
                currentAcc.IsActive = true;

                _unitOfWork.GetRepository<Account>().UpdateAsync(currentAcc);
                await _unitOfWork.CommitAsync();
            }
            return new GiveandtakeResult("Unbanned");
        }

        // Method to change password for the current logged-in account
        public async Task<IGiveandtakeResult> ChangePassword(int accountId, string oldPassword, string newPassword)
        {
            Account account = await _unitOfWork.GetRepository<Account>()
                .SingleOrDefaultAsync(predicate: a => a.AccountId == accountId && a.Password.Equals(oldPassword));

            GiveandtakeResult result = new GiveandtakeResult();

            if (account == null)
            {
                result.Status = -1;
                result.Message = "Incorrect old password";
                return result;
            }

            account.Password = newPassword;

            _unitOfWork.GetRepository<Account>().UpdateAsync(account);
            await _unitOfWork.CommitAsync();

            result.Status = 1;
            result.Message = "Password changed successfully";
            return result;
        }

        // Method to promote an account to Membership
        public async Task<IGiveandtakeResult> PromoteToPremium(int accountId)
        {
            Account account = await _unitOfWork.GetRepository<Account>()
                .SingleOrDefaultAsync(predicate: a => a.AccountId == accountId);
            if (account == null) return new GiveandtakeResult();
            else
            {
                account.IsPremium = true;
                account.PremiumUntil = DateTime.Now.AddDays(30);
                    
                _unitOfWork.GetRepository<Account>().UpdateAsync(account);
                await _unitOfWork.CommitAsync();
            }
            return new GiveandtakeResult("Account was promoted to Premium");
        }
    }

    public class PaginatedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
    #endregion
}