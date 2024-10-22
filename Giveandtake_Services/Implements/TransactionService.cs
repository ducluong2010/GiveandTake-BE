using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Transaction;
using Giveandtake_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Implements
{
    public class TransactionService : ITransactionService
    {
        private readonly TransactionBusiness _transactionBusiness;

        public TransactionService()
        {
            _transactionBusiness = new TransactionBusiness();
        }

        public Task<IGiveandtakeResult> GetAllTransactions()
            => _transactionBusiness.GetAllTransactions();

        public Task<IGiveandtakeResult> GetTransactionByAccount(int accountId)
            => _transactionBusiness.GetTransactionsByAccount(accountId);

        public Task<IGiveandtakeResult> GetTransactionById(int id)
            => _transactionBusiness.GetTransactionById(id);

        public Task<IGiveandtakeResult> GetTransactionsByDonationForSender(int senderAccountId)
          => _transactionBusiness.GetTransactionsByDonationForSender(senderAccountId);

        // Specifid methods for admin and staff

        public Task<IGiveandtakeResult> ChangeTransactionStatusToPending(int transactionId)
            => _transactionBusiness.ChangeTransactionStatusToPending(transactionId);

        public Task<IGiveandtakeResult> ChangeTransactionStatusToSuspended(int transactionId)
            => _transactionBusiness.ChangeTransactionStatusToSuspended(transactionId);

        public Task<IGiveandtakeResult> DeleteSuspendedTransaction(int id)
            => _transactionBusiness.DeleteSuspendedTransaction(id);

        // Specifid methods for user

        public Task<IGiveandtakeResult> CreateTransactionWithDetail(TransactionDTO.CreateTransaction createTransaction, TransactionDetailDTO transactionDetailDto, int senderAccountId)
            => _transactionBusiness.CreateTransactionWithDetail(createTransaction, transactionDetailDto, senderAccountId);

        public Task<IGiveandtakeResult> CompleteTransaction(int transactionId, int senderAccountId)
            => _transactionBusiness.CompleteTransaction(transactionId, senderAccountId);
    }
}
