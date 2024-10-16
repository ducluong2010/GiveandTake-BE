using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GiveandTake_Repo.DTOs.Transaction.TransactionDTO;

namespace Giveandtake_Services.Interfaces
{
    public interface ITransactionService
    {
        Task<IGiveandtakeResult> GetAllTransactions();
        Task<IGiveandtakeResult> GetTransactionById(int id);
        Task<IGiveandtakeResult> GetTransactionByAccount (int accountId);

        // Specifid methods for admin and staff
        Task<IGiveandtakeResult> DeleteSuspendedTransaction(int id);
        Task ChangeTransactionStatusToSuspended(int transactionId);
        Task ChangeTransactionStatusToPending(int transactionId);

        // New methods
        Task<IGiveandtakeResult> CreateTransactionWithDetail(CreateTransaction createTransaction, TransactionDetailDTO transactionDetailDto);
        Task<IGiveandtakeResult> GetTransactionsByDonationForSender(int senderAccountId);

        #region Unused methods
        //Task<IGiveandtakeResult> CreateTransaction(TransactionDTO.CreateTransaction transactionInfo);
        //Task<IGiveandtakeResult> UpdateTransaction(int id, TransactionDTO.UpdateTransaction transactionInfo);
        #endregion
    }
}
