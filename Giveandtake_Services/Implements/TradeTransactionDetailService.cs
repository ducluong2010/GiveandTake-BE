using Giveandtake_Business;
using GiveandTake_Repo.DTOs.TradeTransaction;
using Giveandtake_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Implements
{
    public class TradeTransactionDetailService : ITradeTransactionDetailService
    {
        private readonly TradeTransactionDetailBusiness _tradeTransactionDetailBusiness;
        public TradeTransactionDetailService()
        {
            _tradeTransactionDetailBusiness = new TradeTransactionDetailBusiness();
        }

        public Task<IGiveandtakeResult> CreateTradeTransactionDetail(TradeTransactionDetailDTO tradeDetail)
            => _tradeTransactionDetailBusiness.CreateTradeTransactionDetail(tradeDetail);

        public Task<IGiveandtakeResult> DeleteTradeTransactionDetail(int id)
            => _tradeTransactionDetailBusiness.DeleteTradeTransactionDetail(id);

        public Task<IGiveandtakeResult> GenerateQRCode(int tradeTransactionId, int tradeTransactionDetailId, int requestDonationId)
            => _tradeTransactionDetailBusiness.GenerateQRCode(tradeTransactionId, tradeTransactionDetailId, requestDonationId);

        public Task<IGiveandtakeResult> GetAllTradeTransactionDetail()
            => _tradeTransactionDetailBusiness.GetAllTradeTransactionDetail();

        public Task<IGiveandtakeResult> GetQrcodeByTradeTransactionId(int tradeTransactionId)
            => _tradeTransactionDetailBusiness.GetQrcodeByTradeTransactionId(tradeTransactionId);

        public Task<IGiveandtakeResult> GetTradeTransactionDetailById(int id)
            => _tradeTransactionDetailBusiness.GetTradeTransactionDetailById(id);

        public Task<IGiveandtakeResult> GetTradeTransactionDetailByTradeTransactionId(int tradeId)
            => _tradeTransactionDetailBusiness.GetTradeTransactionDetailByTradeTransactionId(tradeId);
    }
}
