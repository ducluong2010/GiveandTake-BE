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
    public class TransactionDetailService : ITransactionDetailService
    {
        private readonly TransactionDetailBusiness _transactionDetailBusiness;
        public TransactionDetailService()
        {
            _transactionDetailBusiness = new TransactionDetailBusiness();
        }

        public Task<IGiveandtakeResult> CreateTransactionDetail(TransactionDetailDTO transactionDetail)
            => _transactionDetailBusiness.CreateTransactionDetail(transactionDetail);

        public Task<IGiveandtakeResult> DeleteTransactionDetail(int transactionDetailId)
            => _transactionDetailBusiness.DeleteTransactionDetail(transactionDetailId);

        public Task<IGiveandtakeResult> GetAllTransactionDetail()
            => _transactionDetailBusiness.GetAllTransactionDetail();

        public Task<IGiveandtakeResult> GetTransactionDetailById(int transactionDetailId)
            => _transactionDetailBusiness.GetTransactionDetailById(transactionDetailId);

        public Task<IGiveandtakeResult> GetTransactionDetailByTransactionId(int transactionId)
            => _transactionDetailBusiness.GetTransactionDetailByTransactionId(transactionId);

        public Task<IGiveandtakeResult> UpdateTransactionDetail(int transactionDetailId, TransactionDetailDTO transactionDetail)
            => _transactionDetailBusiness.UpdateTransactionDetail(transactionDetailId, transactionDetail);

        public Task<IGiveandtakeResult> GenerateQRCode(int transactionDetailId, int donationid)
            => _transactionDetailBusiness.GenerateQRCode(transactionDetailId, donationid);
    }
}
