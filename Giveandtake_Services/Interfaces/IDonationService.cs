﻿using Giveandtake_Business;
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
        Task<IGiveandtakeResult> GetAllApproved(int page = 1, int pageSize = 8);
        Task<IGiveandtakeResult> GetAllByStaff(int accountId, int page = 1, int pageSize = 8);
        Task<IGiveandtakeResult> GetAllByStaffV2(int accountId, int page = 1, int pageSize = 8);
        Task<IGiveandtakeResult> GetDonationsByAccountId(int accountId);
        Task<IGiveandtakeResult> GetAllByAccountId(int accountId);
        Task<IGiveandtakeResult> GetByAccountIdAndType(int accountId, int type);
        Task<IGiveandtakeResult> UpdateDonation(int id, CreateUpdateDonationDTO donationInfo);
        Task<IGiveandtakeResult> CreateDonation(CreateDonationDTO donationInfo);
        Task<IGiveandtakeResult> DeleteDonation(int id);
        Task<IGiveandtakeResult> ToggleDonationStatus(int id);
        Task<IGiveandtakeResult> ToggleCancel(int id);
        Task<IGiveandtakeResult> ToggleApproved(int id);
        Task<IGiveandtakeResult> ToggleType(int id);
        Task<IGiveandtakeResult> ToggleType2(int id);
        Task<IGiveandtakeResult> ToggleType3(int id);
        Task<IGiveandtakeResult> CheckAndUpdateAllBannedAccountsDonations();
        Task<IGiveandtakeResult> CheckAndUpdateDonationsForActivatedAccounts();
        Task<IGiveandtakeResult> SearchDonations(string searchTerm, int page = 1, int pageSize = 8);
        Task<IGiveandtakeResult> ChangeDonationStatus(int donationId, string newStatus);
        Task<IGiveandtakeResult> GetApprovedDonationByAccountAndType(int accountId);
    }
}
