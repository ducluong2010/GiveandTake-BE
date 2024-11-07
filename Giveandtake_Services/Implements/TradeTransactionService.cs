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

        public Task<IGiveandtakeResult> GetAllTradeTransaction()
        => _tradeTransactionBusiness.GetAllTradeTransaction();

    //    public Task<IGiveandtakeResult> GetTradeTransactionById(int id)
    //    => _tradeTransactionBusiness.GetTradeTransactionById(id); 
    }
}
