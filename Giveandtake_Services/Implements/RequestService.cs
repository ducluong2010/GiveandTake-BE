using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Request;
using Giveandtake_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Implements
{
    public class RequestService : IRequestService
    {
        private readonly RequestBusiness _requestBusiness;
        public RequestService()
        {
            _requestBusiness = new RequestBusiness();
        }

        public Task<IGiveandtakeResult> CancelRequest(int requestId, int receiverId)
            => _requestBusiness.CancelRequest(requestId, receiverId);

        public Task<IGiveandtakeResult> CreateRequest(RequestDTO requestDTO)
            => _requestBusiness.CreateRequest(requestDTO);

        public Task<IGiveandtakeResult> DeleteRequest(int requestId)
            => _requestBusiness.DeleteRequest(requestId);

        public Task<IGiveandtakeResult> GetAllRequests()
            => _requestBusiness.GetAllRequests();

        public Task<IGiveandtakeResult> GetRequestByAccountId(int accountId)
            => _requestBusiness.GetRequestByAccountId(accountId);

        public Task<IGiveandtakeResult> GetRequestByDonationId(int donationId)
            => _requestBusiness.GetRequestByDonationId(donationId);

        public Task<IGiveandtakeResult> GetRequestById(int requestId)
            => _requestBusiness.GetRequestById(requestId);
    }
}
