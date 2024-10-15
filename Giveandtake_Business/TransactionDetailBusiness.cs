using GiveandTake_Repo.DTOs.Transaction;
using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Business
{
    public class TransactionDetailBusiness
    {
        private readonly UnitOfWork _unitOfWork;

        public TransactionDetailBusiness()
        {
            _unitOfWork = new UnitOfWork();
        }

        #region TransactionDetail

        // Get all transaction details
        public async Task<IGiveandtakeResult> GetAllTransactionDetail()
        {
            var transactionDetailList = await _unitOfWork.GetRepository<TransactionDetail>()
                .GetListAsync(selector: x => new GetTransactionDetailDTO
                {
                    TransactionDetailId = x.TransactionDetailId,
                    TransactionId = x.TransactionId,
                    DonationId = x.DonationId
                });
            return new GiveandtakeResult(transactionDetailList);
        }

        // Get transaction detail by its id
        public async Task<IGiveandtakeResult> GetTransactionDetailById(int transactionDetailId)
        {
            var transactionDetail = await _unitOfWork.GetRepository<TransactionDetail>()
                .SingleOrDefaultAsync(predicate: c => c.TransactionDetailId == transactionDetailId,
                                      selector: x => new GetTransactionDetailDTO
                                      {
                                          TransactionDetailId = x.TransactionDetailId,
                                          TransactionId = x.TransactionId,
                                          DonationId = x.DonationId
                                      });
            return new GiveandtakeResult(transactionDetail);
        }

        // Get transaction detail by transaction id
        public async Task<IGiveandtakeResult> GetTransactionDetailByTransactionId(int transactionId)
        {
            var transactionDetail = await _unitOfWork.GetRepository<TransactionDetail>()
                .GetListAsync(predicate: c => c.TransactionId == transactionId,
                              selector: x => new GetTransactionDetailDTO
                              {
                                  TransactionDetailId = x.TransactionDetailId,
                                  TransactionId = x.TransactionId,
                                  DonationId = x.DonationId
                              });
            return new GiveandtakeResult(transactionDetail);
        }

        // Create a new transaction detail
        public async Task<IGiveandtakeResult> CreateTransactionDetail(TransactionDetailDTO transactionDetail)
        {
            var newTransactionDetail = new TransactionDetail
            {
                TransactionId = transactionDetail.TransactionId,
                DonationId = transactionDetail.DonationId
            };

            await _unitOfWork.GetRepository<TransactionDetail>().InsertAsync(newTransactionDetail);

            GiveandtakeResult result = new GiveandtakeResult();

            bool status = await _unitOfWork.CommitAsync() > 0;
            if (status)
            {
                result.Status = 1;
                result.Message = "Transaction Detail created successfully";
            }
            else
            {
                result.Status = -1;
                result.Message = "Transaction Detail creation failed";
            }
            return result;
        }

        // Update a transaction detail
        public async Task<IGiveandtakeResult> UpdateTransactionDetail(int transactionDetailId, TransactionDetailDTO transactionDetail)
        {
            var transactionDetailToUpdate = await _unitOfWork.GetRepository<TransactionDetail>().SingleOrDefaultAsync(predicate: c => c.TransactionDetailId == transactionDetailId);

            if (transactionDetailToUpdate == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Transaction Detail not found"
                };
            }

            transactionDetailToUpdate.TransactionId = transactionDetail.TransactionId;
            transactionDetailToUpdate.DonationId = transactionDetail.DonationId;

            _unitOfWork.GetRepository<TransactionDetail>().UpdateAsync(transactionDetailToUpdate);

            GiveandtakeResult result = new GiveandtakeResult();

            bool status = await _unitOfWork.CommitAsync() > 0;
            if (status)
            {
                result.Status = 1;
                result.Message = "Transaction Detail updated successfully";
            }
            else
            {
                result.Status = -1;
                result.Message = "Transaction Detail update failed";
            }
            return result;
        }

        // Delete a transaction detail
        public async Task<IGiveandtakeResult> DeleteTransactionDetail(int transactionDetailId)
        {
            TransactionDetail transactionDetail = await _unitOfWork.GetRepository<TransactionDetail>()
                .SingleOrDefaultAsync(predicate: c => c.TransactionDetailId == transactionDetailId);
            if (transactionDetail == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Transaction Detail not found"
                };
            }
            else
            {
                _unitOfWork.GetRepository<TransactionDetail>().DeleteAsync(transactionDetail);
                await _unitOfWork.CommitAsync();
            }
            return new GiveandtakeResult(1, "Delete Successfully");
        }

        #endregion TransactionDetail
    }
}
