﻿using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Interfaces
{
    public interface IAccountService
    {
        Task<IGiveandtakeResult> Register(string email, string password);

        Task<IGiveandtakeResult> Login(string email, string password);

        Task<IGiveandtakeResult> GetAllAccount(int page = 1, int pageSize = 8);

        Task<IGiveandtakeResult> GetAccountInfo(int accountId);

        Task<IGiveandtakeResult> GetAccountInfoByEmail(string email);

        Task<IGiveandtakeResult> CreateAccount(AccountDTO inputedAccount);

        Task<IGiveandtakeResult> UpdateAccountInfo(int id, AccountDTO accInfo);

        Task<IGiveandtakeResult> DeleteAccount(int id);

        Task<IGiveandtakeResult> BanAccount(int accountId);
    }
}
