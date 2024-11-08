using Giveandtake_Business;
using Giveandtake_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Implements
{
    public class TradeTransactionService : ITradeTransactionService
    {
        private readonly TradeTransactionBusiness _tradeTransactionBusiness;
        public TradeTransactionService()
        {
            _tradeTransactionBusiness = new TradeTransactionBusiness();
        }

        public Task<IGiveandtakeResult> AcceptTradeRequest(int tradeRequestId, int loggedInAccountId)
         => _tradeTransactionBusiness.AcceptTradeRequest(tradeRequestId, loggedInAccountId);

        public Task<IGiveandtakeResult> CompleteTradeTransaction(int tradeTransactionId, int loggedInAccountId)
        => _tradeTransactionBusiness.CompleteTradeTransaction(tradeTransactionId, loggedInAccountId);

        public Task<IGiveandtakeResult> GetAllTradeTransaction()
        => _tradeTransactionBusiness.GetAllTradeTransaction();

        public Task<IGiveandtakeResult> GetTradeTransactionByAccountId(int accountId)
        => _tradeTransactionBusiness.GetTradeTransactionByAccountId(accountId);

        public Task<IGiveandtakeResult> GetTradeTransactionById(int id)
        => _tradeTransactionBusiness.GetTradeTransactionById(id);

        public Task<IGiveandtakeResult> GetTradeTransactionStatus(int id)
        => _tradeTransactionBusiness.GetTradeTransactionStatus(id);

        public Task<IGiveandtakeResult> RejectTradeRequest(int tradeRequestId, int loggedInAccountId)
        => _tradeTransactionBusiness.RejectTradeRequest(tradeRequestId, loggedInAccountId);
    }
}
