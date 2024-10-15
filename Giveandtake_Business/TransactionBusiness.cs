using GiveandTake_Repo.DTOs.Transaction;
using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static GiveandTake_Repo.DTOs.Transaction.TransactionDTO;
using Transaction = GiveandTake_Repo.Models.Transaction;

namespace Giveandtake_Business
{
    public class TransactionBusiness
    {
        private readonly UnitOfWork _unitOfWork;

        public TransactionBusiness()
        {
            _unitOfWork ??= new UnitOfWork();
        }

        #region CRUD Transaction

        // Get all transactions
        public async Task<IGiveandtakeResult> GetAllTransactions()
        {
            var transactionsList = await _unitOfWork.GetRepository<Transaction>().GetListAsync(
                selector: o => new GetTransaction()
                {
                    TransactionId = o.TransactionId,
                    TotalPoint = o.TotalPoint,
                    CreatedDate = o.CreatedDate,
                    UpdatedDate = o.UpdatedDate,
                    Status = o.Status,
                    AccountId = o.AccountId
                });
            return new GiveandtakeResult(transactionsList);
        }

        // Get transaction by id
        public async Task<IGiveandtakeResult> GetTransactionById(int id)
        {
            var transactionsList = await _unitOfWork.GetRepository<Transaction>().SingleOrDefaultAsync(
                predicate: o => o.TransactionId == id,
                selector: o => new GetTransaction()
                {
                    TransactionId = o.TransactionId,
                    TotalPoint = o.TotalPoint,
                    CreatedDate = o.CreatedDate,
                    UpdatedDate = o.UpdatedDate,
                    Status = o.Status,
                    AccountId = o.AccountId
                });

            if (transactionsList == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Transaction not found"
                };
            }

            return new GiveandtakeResult(transactionsList);
        }
        
        // Get transactions by account
        public async Task<IGiveandtakeResult> GetTransactionsByAccount(int id)
        {
            var transactionsList = await _unitOfWork.GetRepository<Transaction>().GetListAsync(
                 predicate: o => o.AccountId == id,
                 selector: o => new GetTransaction()
                 {
                     TransactionId = o.TransactionId,
                     TotalPoint = o.TotalPoint,
                     CreatedDate = o.CreatedDate,
                     UpdatedDate = o.UpdatedDate,
                     Status = o.Status,
                     AccountId = o.AccountId
                 });
            if (transactionsList == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Transaction not found"
                };
            }
            return new GiveandtakeResult(transactionsList);
        }

        // Create transaction
        public async Task<IGiveandtakeResult> CreateTransaction(CreateTransaction createTransaction)
        {
            Transaction transaction = new Transaction
            {
                TotalPoint = createTransaction.TotalPoint,
                CreatedDate = createTransaction.CreatedDate,
                UpdatedDate = createTransaction.UpdatedDate,
                Status = "Pending",
                AccountId = createTransaction.AccountId
            };

            await _unitOfWork.GetRepository<Transaction>().InsertAsync(transaction);
            IGiveandtakeResult result = new GiveandtakeResult();

            bool status = await _unitOfWork.CommitAsync() > 0;
            if (status)
            {
                result.Status = 1;
                result.Message = "Transaction created successfully";
            }
            else
            {
                result.Status = -1;
                result.Message = "Transaction created failed";
            }
            return result;
        }

        // Change transaction status
        public async Task ChangeTransactionStatus(int transactionId, string status)
        {
            var transaction = await _unitOfWork.GetRepository<Transaction>().SingleOrDefaultAsync(
                predicate: o => o.TransactionId == transactionId);

            if (transaction != null)
            {
                transaction.Status = status;
                transaction.UpdatedDate = DateTime.Now;
                _unitOfWork.GetRepository<Transaction>().UpdateAsync(transaction);
                await _unitOfWork.CommitAsync();
            }
        }

        // Update transaction
        public async Task<IGiveandtakeResult> UpdateTransaction(int id, UpdateTransaction updateTransaction)
        {
            var transaction = await _unitOfWork.GetRepository<Transaction>().SingleOrDefaultAsync(
                predicate: o => o.TransactionId == id);

            if (transaction == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Transaction not found"
                };
            }

            transaction.TotalPoint = updateTransaction.TotalPoint;
            transaction.CreatedDate = updateTransaction.CreatedDate;
            transaction.UpdatedDate = updateTransaction.UpdatedDate;
            transaction.Status = updateTransaction.Status;
            transaction.AccountId = updateTransaction.AccountId;

            _unitOfWork.GetRepository<Transaction>().UpdateAsync(transaction);
            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult
            {
                Status = 1,
                Message = "Transaction updated successfully"
            };
        }

        // Delete transaction
        public async Task<IGiveandtakeResult> DeleteTransaction(int id)
        {
            var transaction = await _unitOfWork.GetRepository<Transaction>().SingleOrDefaultAsync(
                predicate: o => o.TransactionId == id);

            if (transaction == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Transaction not found"
                };
            }

            _unitOfWork.GetRepository<Transaction>().DeleteAsync(transaction);
            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult
            {
                Status = 1,
                Message = "Transaction deleted successfully"
            };
        }

        #endregion

        #region Logic Transaction
        // Tạo transaction và transaction detail khi người nhận tạo yêu cầu
        public async Task<IGiveandtakeResult> CreateTransactionWithDetail(CreateTransaction createTransaction, TransactionDetailDTO transactionDetailDto)
        {
            // Lấy donation dựa trên donationId
            var donation = await _unitOfWork.GetRepository<Donation>().SingleOrDefaultAsync(
                predicate: d => d.DonationId == transactionDetailDto.DonationId);

            if (donation == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Donation not found"
                };
            }

            // Tạo transaction với TotalPoint lấy từ Donation
            Transaction transaction = new Transaction
            {
                TotalPoint = donation.Point, // Cập nhật TotalPoint từ điểm của donation
                CreatedDate = createTransaction.CreatedDate,
                UpdatedDate = createTransaction.UpdatedDate,
                Status = "Pending",
                AccountId = createTransaction.AccountId
            };

            await _unitOfWork.GetRepository<Transaction>().InsertAsync(transaction);
            await _unitOfWork.CommitAsync(); // Commit trước để TransactionId được tạo

            transactionDetailDto.TransactionId = transaction.TransactionId;

            // Tạo transaction detail
            TransactionDetail transactionDetail = new TransactionDetail
            {
                TransactionId = transaction.TransactionId,
                DonationId = transactionDetailDto.DonationId,
            };

            await _unitOfWork.GetRepository<TransactionDetail>().InsertAsync(transactionDetail);

            IGiveandtakeResult result = new GiveandtakeResult();

            bool status = await _unitOfWork.CommitAsync() > 0;
            if (status)
            {
                result.Status = 1;
                result.Message = "Transaction and TransactionDetail created successfully";
            }
            else
            {
                result.Status = -1;
                result.Message = "Transaction creation failed";
            }
            return result;
        }


        // Lấy các giao dịch chứa transaction detail với donation id của người gửi
        public async Task<IGiveandtakeResult> GetTransactionsByDonationForSender(int senderAccountId)
        {
            var transactionsList = await _unitOfWork.GetRepository<Transaction>()
                .GetListAsync(
                    predicate: t => t.TransactionDetails.Any(td =>
                        td.Donation != null && td.Donation.AccountId == senderAccountId),
                    selector: t => new GetTransaction()
                    {
                        TransactionId = t.TransactionId,
                        TotalPoint = t.TotalPoint,
                        CreatedDate = t.CreatedDate,
                        UpdatedDate = t.UpdatedDate,
                        Status = t.Status,
                        AccountId = t.AccountId
                    });

            if (transactionsList == null || !transactionsList.Any())
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "No transactions found for your donations"
                };
            }

            return new GiveandtakeResult(transactionsList);
        }
        #endregion
    }
}
