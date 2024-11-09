using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Request;
using GiveandTake_Repo.Repository.Implements;
using Giveandtake_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Implements
{
    public class TradeRequestService : ITradeRequestService
    {
        private readonly TradeRequestBusiness _tradeRequestBusiness;
        public TradeRequestService()
        {
            _tradeRequestBusiness = new TradeRequestBusiness();
        }

        public Task<IGiveandtakeResult> CancelTradeRequest(int requestId, int accountId)
            => _tradeRequestBusiness.CancelTradeRequest(requestId, accountId);

        public Task<IGiveandtakeResult> CreateTradeRequest(TradeRequestDTO tradeRequestDTO)
            => _tradeRequestBusiness.CreateTradeRequest(tradeRequestDTO);

        public Task<IGiveandtakeResult> DeleteTradeRequest(int requestId)
            => _tradeRequestBusiness.DeleteTradeRequest(requestId);

        public Task<IGiveandtakeResult> GetAllTradeRequests()
            => _tradeRequestBusiness.GetAllTradeRequests();

        public Task<IGiveandtakeResult> GetTradeRequestByAccountId(int accountId)
            => _tradeRequestBusiness.GetTradeRequestByAccountId(accountId);

        public Task<IGiveandtakeResult> GetTradeRequestById(int id)
            => _tradeRequestBusiness.GetTradeRequestById(id);

        public Task<IGiveandtakeResult> GetTradeRequestByTradeDonationId(int requestDonationId)
            => _tradeRequestBusiness.GetTradeRequestByTradeDonationId(requestDonationId);
    }
}
