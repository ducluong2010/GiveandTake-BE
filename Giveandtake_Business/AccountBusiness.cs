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
        // Method for user login
        public async Task<IGiveandtakeResult> Login(string email, string password)
        {
            Account account = await _unitOfWork.GetRepository<Account>()
                .SingleOrDefaultAsync(predicate: a => a.Email.Equals(email) && a.Password.Equals(password));

            GiveandtakeResult result = new GiveandtakeResult();

            if (account == null)
            {
                result.Status = -1;
                result.Message = "Incorrect Email or Password";
                return result;
            }

            if (!(bool)account.IsActive)
            {
                result.Status = -1;
                result.Message = "Your account is currently inactive (banned)";
                return result;
            }

            result.Data = JwtUtils.GenerateJwtToken(account);
            result.Status = 1;
            result.Message = "Login successfully";
            return result;
        }

        // Method for user registration
        public async Task<IGiveandtakeResult> Register(string email, string password)
        {
            Account account = await _unitOfWork.GetRepository<Account>()
                .SingleOrDefaultAsync(predicate: a => a.Email.Equals(email));

            GiveandtakeResult result = new GiveandtakeResult();

            if (account != null)
            {
                result.Status = -1;
                result.Message = "Email has already used";
                return result;
            }

            Account newAccount = new Account
            {
                AccountId = 0,
                FullName = String.Empty,
                Password = password,
                Email = email,
                Phone = String.Empty,
                Address = String.Empty,
                Point = 0,
                AvatarUrl = String.Empty,
                RoleId = 3,
                IsActive = true,
            };

            await _unitOfWork.GetRepository<Account>().InsertAsync(newAccount);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccessful)
            {
                result.Status = -1;
                result.Message = "Register unsuccessfully";
            }
            else
            {
                result.Data = JwtUtils.GenerateJwtToken(newAccount);
                result.Status = 1;
                result.Message = "Register successfully";
            }

            return result;
        }
        #endregion


        #region Account
        // Method to get all active accounts
        public async Task<IGiveandtakeResult> GetAllAccount()
        {
            var accList = await _unitOfWork.GetRepository<Account>()
                .GetListAsync(predicate: a => (bool)a.IsActive,
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
                                  RoleId = a.RoleId
                              });
            return new GiveandtakeResult(accList);
        }

        // Method to get account information by account ID
        public async Task<IGiveandtakeResult> GetAccountInfo(int accountId)
        {
            var acc = await _unitOfWork.GetRepository<Account>()
                                //.SingleOrDefaultAsync(predicate: a => a.AccountId == accountId && (bool)a.IsActive,
                                .SingleOrDefaultAsync(predicate: a => a.AccountId == accountId,

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
                                          RoleId = a.RoleId
                                      });
            if (acc != null) return new GiveandtakeResult(acc);
            return new GiveandtakeResult();
        }

        // Method to get account information by email
        public async Task<IGiveandtakeResult> GetAccountInfoByEmail(string email)
        {
            var acc = await _unitOfWork.GetRepository<Account>()
                                //.SingleOrDefaultAsync(predicate: a => a.Email.Equals(email) && (bool)a.IsActive,
                                .SingleOrDefaultAsync(predicate: a => a.Email.Equals(email),

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
                                          RoleId = a.RoleId
                                      });
            if (acc != null) return new GiveandtakeResult(acc);
            return new GiveandtakeResult();
        }



        // Method to create a new account (used by admin)
        public async Task<IGiveandtakeResult> CreateAccount(AccountDTO inputedAccount)
        {
            Account account = await _unitOfWork.GetRepository<Account>()
                .SingleOrDefaultAsync(predicate: a => a.Email.Equals(inputedAccount.Email));

            GiveandtakeResult result = new GiveandtakeResult();

            if (account != null)
            {
                result.Status = -1;
                result.Message = "Email has already used";
                return result;
            }

            Account newAccount = new Account
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
            };

            await _unitOfWork.GetRepository<Account>().InsertAsync(newAccount);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccessful)
            {
                result.Status = -1;
                result.Message = "Create unsuccessfully";
            }
            else
            {
                result = new GiveandtakeResult(1, "Create Susscessfull");
            }

            return result;
        }

        // Method to update account information
        public async Task<IGiveandtakeResult> UpdateAccountInfo(int id, AccountDTO accInfo)
        {
            Account currentAcc = await _unitOfWork.GetRepository<Account>()
                .SingleOrDefaultAsync(predicate: a => a.AccountId == id);
            if (currentAcc == null) return new GiveandtakeResult(-1, "Account cannot be found");
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
            if (currentAcc == null) return new GiveandtakeResult(-1, "Account cannot be found");
            else
            {
                _unitOfWork.GetRepository<Account>().DeleteAsync(currentAcc);
                await _unitOfWork.CommitAsync();
            }

            return new GiveandtakeResult("Delete Successfull");
        }

        // Method to ban an account
        public async Task<IGiveandtakeResult> BanAccount(int accountId)
        {
            Account account = await _unitOfWork.GetRepository<Account>()
                .SingleOrDefaultAsync(predicate: a => a.AccountId == accountId);
            if (account == null) return new GiveandtakeResult();
            else
            {
                account.IsActive = false;

                _unitOfWork.GetRepository<Account>().UpdateAsync(account);
                await _unitOfWork.CommitAsync();
            }
            return new GiveandtakeResult("Banned");
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
        #endregion
    }
}