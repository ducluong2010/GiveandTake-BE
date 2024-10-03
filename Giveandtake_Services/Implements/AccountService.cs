using Giveandtake_Business;
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

        public Task<IGiveandtakeResult> Register(string email, string password)
            => _accountBusiness.Register(email, password);

        public Task<IGiveandtakeResult> UnbanAccount(int accountId)
            => _accountBusiness.UnbanAccount(accountId);

        public Task<IGiveandtakeResult> UpdateAccountInfo(int id, AccountDTO accInfo)
            => _accountBusiness.UpdateAccountInfo(id, accInfo);
    }
}
