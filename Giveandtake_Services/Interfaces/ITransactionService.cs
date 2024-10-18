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
        Task<IGiveandtakeResult> GetTransactionsByDonationForSender(int senderAccountId);

        // Specifid methods for admin and staff
        Task<IGiveandtakeResult> DeleteSuspendedTransaction(int id);
        Task<IGiveandtakeResult> ChangeTransactionStatusToSuspended(int transactionId);
        Task<IGiveandtakeResult> ChangeTransactionStatusToPending(int transactionId);

        // Specifid methods for user
        Task<IGiveandtakeResult> CreateTransactionWithDetail(CreateTransaction createTransaction, TransactionDetailDTO transactionDetailDto, int senderAccountId);
        //Task<IGiveandtakeResult> ChangeTransactionStatusToAccepted(int transactionId, int senderAccountId);
        //Task<IGiveandtakeResult> ChangeTransactionStatusToRejected(int transactionId, int senderAccountId);
        Task<IGiveandtakeResult> CompleteTransaction(int transactionId, int senderAccountId);
    }
}
