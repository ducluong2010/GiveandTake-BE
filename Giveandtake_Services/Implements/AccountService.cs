﻿using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Account;
using Giveandtake_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Implements
{
    public class AccountService : IAccountService
    {
        private readonly AccountBusiness _accountBusiness;

        public AccountService()
        {
           _accountBusiness = new AccountBusiness();
        }

        public Task<IGiveandtakeResult> BanAccount(int accountId)
            => _accountBusiness.BanAccount(accountId);

        public Task<IGiveandtakeResult> CreateAccount(AccountDTO inputedAccount)
            => _accountBusiness.CreateAccount(inputedAccount);

        public Task<IGiveandtakeResult> DeleteAccount(int id)
            => _accountBusiness.DeleteAccount(id);

        public Task<IGiveandtakeResult> GetAccountInfo(int accountId)
            => _accountBusiness.GetAccountInfo(accountId);

        public Task<IGiveandtakeResult> GetAccountInfoByEmail(string email)
            => _accountBusiness.GetAccountInfoByEmail(email);

        public Task<IGiveandtakeResult> GetAllAccount(int page = 1, int pageSize = 8)
            => _accountBusiness.GetAllAccount(page, pageSize);

        public Task<IGiveandtakeResult> Login(string email, string password)
            => _accountBusiness.Login(email, password);

        public Task<IGiveandtakeResult> PromoteToPremium(int accountId)
            => _accountBusiness.PromoteToPremium(accountId);


        public Task<IGiveandtakeResult> UnbanAccount(int accountId)
            => _accountBusiness.UnbanAccount(accountId);

        public Task<IGiveandtakeResult> UpdateAccountInfo(int id, AccountDTO accInfo)
            => _accountBusiness.UpdateAccountInfo(id, accInfo);

        public Task<IGiveandtakeResult> ChangePassword(int accountId, string oldPassword, string newPassword)
            => _accountBusiness.ChangePassword(accountId, oldPassword, newPassword);

        public Task<IGiveandtakeResult> Register(UserRegisterDTO registerDto)
            => _accountBusiness.Register(registerDto);

        public Task<IGiveandtakeResult> UpdatePremiumUntilById(int accountId)
            => _accountBusiness.UpdatePremiumUntilById(accountId);

        public Task<IGiveandtakeResult> Update3MonthsPremiumUntilById(int accountId)
            => _accountBusiness.Update3MonthsPremiumUntilById(accountId);

        public Task<IGiveandtakeResult> Update6MonthsPremiumUntilById(int accountId)
            => _accountBusiness.Update6MonthsPremiumUntilById(accountId);

        public Task<IGiveandtakeResult> GetAllBannedAccount(int page = 1, int pageSize = 8)
            => _accountBusiness.GetAllBannedAccount(page, pageSize);
    }
}
