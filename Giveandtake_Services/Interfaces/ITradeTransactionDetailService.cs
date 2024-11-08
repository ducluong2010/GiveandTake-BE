using Giveandtake_Business;
using GiveandTake_Repo.DTOs.TradeTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Interfaces
{
    public interface ITradeTransactionDetailService
    {
        Task<IGiveandtakeResult> GetAllTradeTransactionDetail();
        Task<IGiveandtakeResult> GetTradeTransactionDetailById(int id);
        Task<IGiveandtakeResult> GetTradeTransactionDetailByTradeTransactionId(int tradeId);
        Task<IGiveandtakeResult> CreateTradeTransactionDetail(TradeTransactionDetailDTO tradeDetail);
        Task<IGiveandtakeResult> DeleteTradeTransactionDetail(int id);
        Task<IGiveandtakeResult> GenerateQRCode(int tradeTransactionId, int tradeTransactionDetailId, int requestDonationId);
        Task<IGiveandtakeResult> GetQrcodeByTradeTransactionId(int tradeTransactionId);
    }
}
