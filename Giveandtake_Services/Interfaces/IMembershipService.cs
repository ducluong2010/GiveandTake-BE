using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Member;
using GiveandTake_Repo.Models;
using Microsoft.AspNetCore.Http;

namespace Giveandtake_Services.Interfaces
{
    public interface IMembershipService
    {
        string CreatePaymentUrlAsync(PaymentInformationModel model, HttpContext context);
        PaymentResponseModel PaymentExecuteAsync(IQueryCollection collections);
        Task UpdateAccountIsPremiumAsync(int accountId);
        Task<PaymentResponseModel> HandlePaymentCallbackAsync(IQueryCollection collections);

        Task<IGiveandtakeResult> GetAllMemberships(int page = 1, int pageSize = 8);
        Task<IGiveandtakeResult> GetMembershipById(int accountId);
        Task<IGiveandtakeResult> CreateMembership(CreateMembershipDTO membershipInfo);
        Task<IGiveandtakeResult> CreateMembership3Months(CreateMembershipDTO membershipInfo);
        Task<IGiveandtakeResult> CreateMembership6Months(CreateMembershipDTO membershipInfo);
        Task<IGiveandtakeResult> UpdateMembership(int id, UpdateMembershipDTO membershipInfo);
        Task<IGiveandtakeResult> DeleteMembership(int id);
    }
}
