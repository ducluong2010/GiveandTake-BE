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

        public Task ChangeTransactionStatus(int id, string status)
            => _transactionBusiness.ChangeTransactionStatus(id, status);

        public Task<IGiveandtakeResult> CreateTransaction(TransactionDTO.CreateTransaction transactionInfo)
            => _transactionBusiness.CreateTransaction(transactionInfo);

        public Task<IGiveandtakeResult> DeleteTransaction(int id)
            => _transactionBusiness.DeleteTransaction(id);

        public Task<IGiveandtakeResult> GetAllTransactions()
            => _transactionBusiness.GetAllTransactions();

        public Task<IGiveandtakeResult> GetTransactionByAccount(int accountId)
            => _transactionBusiness.GetTransactionsByAccount(accountId);

        public Task<IGiveandtakeResult> GetTransactionById(int id)
            => _transactionBusiness.GetTransactionById(id);

        public Task<IGiveandtakeResult> UpdateTransaction(int id, TransactionDTO.UpdateTransaction transactionInfo)
            => _transactionBusiness.UpdateTransaction(id, transactionInfo);
    }
}
