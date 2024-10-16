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

        // New methods

        public Task<IGiveandtakeResult> GetTransactionsByDonationForSender(int senderAccountId)
            => _transactionBusiness.GetTransactionsByDonationForSender(senderAccountId);

        public Task<IGiveandtakeResult> CreateTransactionWithDetail(TransactionDTO.CreateTransaction createTransaction, TransactionDetailDTO transactionDetailDto)
           => _transactionBusiness.CreateTransactionWithDetail(createTransaction, transactionDetailDto);

        // Specifid methods for admin and staff

        public Task ChangeTransactionStatusToPending(int transactionId)
            => _transactionBusiness.ChangeTransactionStatusToPending(transactionId);

        public Task ChangeTransactionStatusToSuspended(int transactionId)
            => _transactionBusiness.ChangeTransactionStatusToSuspended(transactionId);

        public Task<IGiveandtakeResult> DeleteSuspendedTransaction(int id)
            => _transactionBusiness.DeleteSuspendedTransaction(id);

        #region Unused methods
        //public Task<IGiveandtakeResult> CreateTransaction(TransactionDTO.CreateTransaction transactionInfo)
        //    => _transactionBusiness.CreateTransaction(transactionInfo);

        //public Task<IGiveandtakeResult> UpdateTransaction(int id, TransactionDTO.UpdateTransaction transactionInfo)
        //    => _transactionBusiness.UpdateTransaction(id, transactionInfo);
        #endregion
    }
}
