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
    //    Task<IGiveandtakeResult> GetTradeTransactionById(int id);
    }
}
