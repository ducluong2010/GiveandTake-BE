using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Interfaces
{
    public interface IRequestService
    {
        Task<IGiveandtakeResult> GetAllRequests();
        Task<IGiveandtakeResult> GetRequestById(int requestId);
        Task<IGiveandtakeResult> GetRequestByDonationId(int donationId);
        Task<IGiveandtakeResult> GetRequestByAccountId(int accountId);
        Task<IGiveandtakeResult> CreateRequest(RequestDTO requestDTO);
        Task<IGiveandtakeResult> CancelRequest(int requestId, int receiverId);
        Task<IGiveandtakeResult> DeleteRequest(int requestId);
        Task<IGiveandtakeResult> CancelRequestsByDonationId(int donationId);

    }
}
