using Giveandtake_Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Interfaces
{
    public interface ITradeTransactionService
    {
        Task<IGiveandtakeResult> GetAllTradeTransaction();
        Task<IGiveandtakeResult> GetTradeTransactionById(int id);
        Task<IGiveandtakeResult> GetTradeTransactionStatus(int id);
        Task<IGiveandtakeResult> GetTradeTransactionByAccountId(int accountId);
        Task<IGiveandtakeResult> AcceptTradeRequest(int tradeRequestId, int loggedInAccountId);
        Task<IGiveandtakeResult> RejectTradeRequest(int tradeRequestId, int loggedInAccountId);
        Task<IGiveandtakeResult> CompleteTradeTransaction(int tradeTransactionId, int loggedInAccountId);
        Task<IGiveandtakeResult> CancelTradeTransaction(int tradeTransactionId, int loggedInAccountId);
    }
}
