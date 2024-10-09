using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Interfaces
{
    public interface ITransactionService
    {
        Task<IGiveandtakeResult> GetAllTransactions();
        Task<IGiveandtakeResult> GetTransactionById(int id);
        Task<IGiveandtakeResult> GetTransactionByAccount (int accountId);
        Task<IGiveandtakeResult> CreateTransaction(TransactionDTO.CreateTransaction transactionInfo);
        Task<IGiveandtakeResult> UpdateTransaction(int id, TransactionDTO.UpdateTransaction transactionInfo);
        Task<IGiveandtakeResult> DeleteTransaction(int id);
        Task ChangeTransactionStatus(int id, string status);

    }
}
