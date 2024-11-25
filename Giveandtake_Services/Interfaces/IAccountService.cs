using Giveandtake_Business;
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
        Task<IGiveandtakeResult> Register(UserRegisterDTO registerDto);

        Task<IGiveandtakeResult> Login(string email, string password);

        Task<IGiveandtakeResult> GetAllAccount(int page = 1, int pageSize = 8);

        Task<IGiveandtakeResult> GetAllBannedAccount(int page = 1, int pageSize = 8);

        Task<IGiveandtakeResult> GetAccountInfo(int accountId);

        Task<IGiveandtakeResult> GetAccountInfoByEmail(string email);

        Task<IGiveandtakeResult> CreateAccount(AccountDTO inputedAccount);

        Task<IGiveandtakeResult> UpdateAccountInfo(int id, AccountDTO accInfo);

        Task<IGiveandtakeResult> DeleteAccount(int id);

        Task<IGiveandtakeResult> BanAccount(int accountId);

        Task<IGiveandtakeResult> UnbanAccount(int accountId);

        Task<IGiveandtakeResult> PromoteToPremium(int accountId);

        Task<IGiveandtakeResult> ChangePassword(int accountId, string oldPassword, string newPassword);

        Task<IGiveandtakeResult> UpdatePremiumUntilById(int accountId);
        Task<IGiveandtakeResult> Update3MonthsPremiumUntilById(int accountId);
        Task<IGiveandtakeResult> Update6MonthsPremiumUntilById(int accountId);

    }
}
