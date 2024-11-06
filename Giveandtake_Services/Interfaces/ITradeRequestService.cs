using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Interfaces
{
    public interface ITradeRequestService
    {
        Task<IGiveandtakeResult> GetAllTradeRequests();
        Task<IGiveandtakeResult> GetTradeRequestById(int id);
        Task<IGiveandtakeResult> GetTradeRequestByAccountId(int accountId);
        Task<IGiveandtakeResult> CreateTradeRequest(TradeRequestDTO tradeRequestDTO);
        Task<IGiveandtakeResult> CancelTradeRequest(int requestId, int accountId);
        Task<IGiveandtakeResult> DeleteTradeRequest(int requestId);
    }
}
