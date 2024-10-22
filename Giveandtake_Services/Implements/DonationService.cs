using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Donation;
using GiveandTake_Repo.Models;
using Giveandtake_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Implements
{
    public class DonationService : IDonationService
    {
        private readonly DonationBusiness _donationBusiness;

        public DonationService()
        {
            _donationBusiness = new DonationBusiness();
        }

        public Task<IGiveandtakeResult> GetAllDonations(int page = 1, int pageSize = 8)
            => _donationBusiness.GetAllDonations(page, pageSize);

        public Task<IGiveandtakeResult> GetDonationById(int id)
            => _donationBusiness.GetDonationById(id);

        public Task<IGiveandtakeResult> CreateDonation(CreateDonationDTO donationInfo)
            => _donationBusiness.CreateDonation(donationInfo);

        public Task<IGiveandtakeResult> UpdateDonation(int id, CreateUpdateDonationDTO donationInfo)
            => _donationBusiness.UpdateDonation(id, donationInfo);

        public Task<IGiveandtakeResult> DeleteDonation(int id)
            => _donationBusiness.DeleteDonation(id);

        public Task<IGiveandtakeResult> ToggleDonationStatus(int donationId)
            => _donationBusiness.ToggleDonationStatus(donationId);

        public Task<IGiveandtakeResult> ToggleCancel(int donationId)
            => _donationBusiness.ToggleCancel(donationId);
        public Task<IGiveandtakeResult> ToggleApproved(int donationId)
            => _donationBusiness.ToggleApproved(donationId);

        public Task<IGiveandtakeResult> CheckAndUpdateAllBannedAccountsDonations()
           => _donationBusiness.CheckAndUpdateAllBannedAccountsDonations();

        public Task<IGiveandtakeResult> CheckAndUpdateDonationsForActivatedAccounts()
           => _donationBusiness.CheckAndUpdateDonationsForActivatedAccounts();

        public Task<IGiveandtakeResult> SearchDonations(string searchTerm, int page = 1, int pageSize = 8)
           => _donationBusiness.SearchDonations(searchTerm, page, pageSize);

        public Task<IGiveandtakeResult> ChangeDonationStatus(int donationId, string newStatus)
            => _donationBusiness.ChangeDonationStatus(donationId, newStatus);
    }
}
