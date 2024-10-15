using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Interfaces
{
    public interface ITransactionDetailService
    {
        Task<IGiveandtakeResult> GetAllTransactionDetail();
        Task<IGiveandtakeResult> GetTransactionDetailById(int transactionDetailId);
        Task<IGiveandtakeResult> GetTransactionDetailByTransactionId(int transactionId);
        Task<IGiveandtakeResult> CreateTransactionDetail(TransactionDetailDTO transactionDetail);
        Task<IGiveandtakeResult> UpdateTransactionDetail(int transactionDetailId, TransactionDetailDTO transactionDetail);
        Task<IGiveandtakeResult> DeleteTransactionDetail(int transactionDetailId);
    }
}
