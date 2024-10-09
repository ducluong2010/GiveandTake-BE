using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Donation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Interfaces
{
    public interface IDonationService
    {
        Task<IGiveandtakeResult> GetDonationById(int id);
        Task<IGiveandtakeResult> GetAllDonations(int page = 1, int pageSize = 8);
        Task<IGiveandtakeResult> UpdateDonation(int id, CreateUpdateDonationDTO donationInfo);
        Task<IGiveandtakeResult> CreateDonation(CreateDonationDTO donationInfo);
        Task<IGiveandtakeResult> DeleteDonation(int id);
        Task<IGiveandtakeResult> ToggleDonationStatus(int id);
        Task<IGiveandtakeResult> ToggleCancel(int id);
        Task<IGiveandtakeResult> ToggleApproved(int id);
        Task<IGiveandtakeResult> CheckAndUpdateAllBannedAccountsDonations();
        Task<IGiveandtakeResult> CheckAndUpdateDonationsForActivatedAccounts();
    }
}
