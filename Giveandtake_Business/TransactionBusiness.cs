using GiveandTake_Repo.DTOs.Transaction;
using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using Microsoft.EntityFrameworkCore;
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
        private readonly TransactionDetailBusiness _transactionDetailBusiness;


        public TransactionBusiness()
        {
            _unitOfWork ??= new UnitOfWork();
            _transactionDetailBusiness = new TransactionDetailBusiness();

        }

        #region Basic Transaction

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
            var transaction = await _unitOfWork.GetRepository<Transaction>()
                .SingleOrDefaultAsync(
                    predicate: o => o.TransactionId == id,
                    selector: o => new
                    {
                        Transaction = new GetTransaction()
                        {
                            TransactionId = o.TransactionId,
                            TotalPoint = o.TotalPoint,
                            CreatedDate = o.CreatedDate,
                            UpdatedDate = o.UpdatedDate,
                            Status = o.Status,
                            AccountId = o.AccountId
                        },
                        TransactionDetails = o.TransactionDetails.Select(td => new GetTransactionDetailDTO()
                        {
                            TransactionDetailId = td.TransactionDetailId,
                            TransactionId = td.TransactionId,
                            DonationId = td.DonationId,
                            Qrcode = td.Qrcode
                        }).ToList()
                    });

            if (transaction == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Transaction not found"
                };
            }

            return new GiveandtakeResult(new
            {
                Transaction = transaction.Transaction,
                TransactionDetails = transaction.TransactionDetails
            });
        }


        // Get transactions by receiver id
        public async Task<IGiveandtakeResult> GetTransactionsByAccount(int id)
        {
            var transactionsList = await _unitOfWork.GetRepository<Transaction>().GetListAsync(
                 predicate: o => o.AccountId == id,
                 selector: o => new
                 {
                     Transaction = new GetTransaction()
                     {
                         TransactionId = o.TransactionId,
                         TotalPoint = o.TotalPoint,
                         CreatedDate = o.CreatedDate,
                         UpdatedDate = o.UpdatedDate,
                         Status = o.Status,
                         AccountId = o.AccountId
                     },
                     TransactionDetails = o.TransactionDetails.Select(td => new GetTransactionDetailDTO()
                     {
                         TransactionDetailId = td.TransactionDetailId,
                         TransactionId = td.TransactionId,
                         DonationId = td.DonationId,
                         Qrcode = td.Qrcode
                     }).ToList()
                 });

            if (transactionsList == null || !transactionsList.Any())
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Transaction not found"
                };
            }

            return new GiveandtakeResult(transactionsList);
        }


        public async Task<IGiveandtakeResult> GetTransactionsByDonationForSender(int senderAccountId)
        {
            var transactionsList = await _unitOfWork.GetRepository<Transaction>()
                .GetListAsync(
                    predicate: t => t.TransactionDetails.Any(td =>
                        td.Donation != null && td.Donation.AccountId == senderAccountId),
                    selector: t => new
                    {
                        Transaction = new GetTransaction()
                        {
                            TransactionId = t.TransactionId,
                            TotalPoint = t.TotalPoint,
                            CreatedDate = t.CreatedDate,
                            UpdatedDate = t.UpdatedDate,
                            Status = t.Status,
                            AccountId = t.AccountId
                        },
                        TransactionDetails = t.TransactionDetails.Select(td => new GetTransactionDetailDTO()
                        {
                            TransactionDetailId = td.TransactionDetailId,
                            TransactionId = td.TransactionId,
                            DonationId = td.DonationId,
                            Qrcode = td.Qrcode
                        }).ToList()
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

        #region Specific Admin Transaction

        // Change transaction status to "Suspended" - Admin/Staff
        public async Task<IGiveandtakeResult> ChangeTransactionStatusToSuspended(int transactionId)
        {
            Transaction transaction = await _unitOfWork.GetRepository<Transaction>().SingleOrDefaultAsync(
                predicate: o => o.TransactionId == transactionId);

            if(transaction == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Transaction not found"
                };
            }
            else
            {
                transaction.Status = "Suspended";
                transaction.UpdatedDate = DateTime.Now;
                _unitOfWork.GetRepository<Transaction>().UpdateAsync(transaction);
                await _unitOfWork.CommitAsync();
            }
            return new GiveandtakeResult
            {
                Status = 1,
                Message = "Transaction status changed to Suspended"
            };
        }

        // Revert transaction status to "Pending" - Admin/Staff
        public async Task<IGiveandtakeResult> ChangeTransactionStatusToPending(int transactionId)
        {
            Transaction transaction = await _unitOfWork.GetRepository<Transaction>().SingleOrDefaultAsync(
                predicate: o => o.TransactionId == transactionId);

            if (transaction == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Transaction not found"
                };
            }

            if (transaction.Status == "Suspended")
            {
                transaction.Status = "Pending";
                transaction.UpdatedDate = DateTime.Now;
                _unitOfWork.GetRepository<Transaction>().UpdateAsync(transaction);
                await _unitOfWork.CommitAsync();

                return new GiveandtakeResult
                {
                    Status = 1,
                    Message = "Transaction status changed to Pending"
                };
            }
            else
            {
                throw new InvalidOperationException("Transaction is not suspended and cannot be reverted to pending.");
            }
        }

        // Delete suspended transaction and its details - Admin/Staff
        public async Task<IGiveandtakeResult> DeleteSuspendedTransaction(int id)
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

            if (transaction.Status != "Suspended")
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Only suspended transactions can be deleted"
                };
            }

            var transactionDetails = await _unitOfWork.GetRepository<TransactionDetail>().FindAllAsync(
                predicate: td => td.TransactionId == id);

            foreach (var detail in transactionDetails)
            {
                _unitOfWork.GetRepository<TransactionDetail>().DeleteAsync(detail);
            }

            _unitOfWork.GetRepository<Transaction>().DeleteAsync(transaction);
            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult
            {
                Status = 1,
                Message = "Transaction and its details deleted successfully"
            };
        }

        #endregion

        #region Specific User Transaction

        //// Create transaction and transaction detail at the same time - Sender
        public async Task<IGiveandtakeResult> CreateTransactionWithDetail(CreateTransaction createTransaction,
            TransactionDetailDTO transactionDetailDto, int senderAccountId)
        {
            // Kiểm tra tài khoản
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: a => a.AccountId == createTransaction.AccountId);

            if (account == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Account not found"
                };
            }

            // Kiểm tra donation
            var donation = await _unitOfWork.GetRepository<Donation>().SingleOrDefaultAsync(
                predicate: d => d.DonationId == transactionDetailDto.DonationId && d.AccountId == senderAccountId);

            if (donation == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Donation not found or it does not belong to you"
                };
            }

            if (donation.Status != "Approved")
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Transaction cannot be created with a donation that is not approved."
                };
            }

            // Cập nhật trạng thái donation
            donation.Status = "Hiding";
            _unitOfWork.GetRepository<Donation>().UpdateAsync(donation);

            // Kiểm tra yêu cầu
            var request = await _unitOfWork.GetRepository<Request>().SingleOrDefaultAsync(
                predicate: r => r.DonationId == donation.DonationId && r.AccountId == createTransaction.AccountId);

            if (request == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Receiver has not made a request for this donation."
                };
            }

            // Tạo transaction
            Transaction transaction = new Transaction
            {
                TotalPoint = donation.Point,
                CreatedDate = createTransaction.CreatedDate,
                UpdatedDate = createTransaction.UpdatedDate,
                Status = "Pending",
                AccountId = createTransaction.AccountId
            };

            await _unitOfWork.GetRepository<Transaction>().InsertAsync(transaction);
            await _unitOfWork.CommitAsync(); // Commit để có TransactionId

            // Tạo transaction detail
            transactionDetailDto.TransactionId = transaction.TransactionId;
            TransactionDetail transactionDetail = new TransactionDetail
            {
                TransactionId = transaction.TransactionId,
                DonationId = transactionDetailDto.DonationId,
                Qrcode = null // QRCode sẽ được tạo sau
            };

            await _unitOfWork.GetRepository<TransactionDetail>().InsertAsync(transactionDetail);
            await _unitOfWork.CommitAsync(); // Commit để có TransactionDetailId

            // Tạo QRCode và cập nhật TransactionDetail
            var qrCodeResult = await _transactionDetailBusiness.GenerateQRCode(transaction.TransactionId, transactionDetail.TransactionDetailId, (int)transactionDetailDto.DonationId);

            if (qrCodeResult.Status < 0)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = qrCodeResult.Message
                };
            }

            // Gán QR code URL từ qrCodeResult.Data
            transactionDetail.Qrcode = qrCodeResult.Data.ToString();
            _unitOfWork.GetRepository<TransactionDetail>().UpdateAsync(transactionDetail);

            // Cập nhật trạng thái request
            request.Status = "Accepted";
            _unitOfWork.GetRepository<Request>().UpdateAsync(request);

            // Từ chối các yêu cầu khác
            var otherRequests = await _unitOfWork.GetRepository<Request>().GetListAsync(
                predicate: r => r.DonationId == donation.DonationId && r.RequestId != request.RequestId);

            foreach (var otherRequest in otherRequests)
            {
                otherRequest.Status = "Rejected";
                _unitOfWork.GetRepository<Request>().UpdateAsync(otherRequest);
            }

            await _unitOfWork.CommitAsync();

            // Trả về kết quả thành công cùng với TransactionId và Qrcode
            return new GiveandtakeResult
            {
                Status = 1,
                Message = "Transaction and TransactionDetail created successfully, QR Code generated, and other requests rejected.",
                Data = new
                {
                    TransactionId = transaction.TransactionId,
                    AccountId = transaction.AccountId,
                    DonationId = transactionDetail.DonationId,
                    Qrcode = transactionDetail.Qrcode // Trả về URL QRCode
                }
            };
        }


        // Complete the transaction - Sender
        public async Task<IGiveandtakeResult> CompleteTransaction(int transactionId, int senderAccountId)
        {
            var transaction = await _unitOfWork.GetRepository<Transaction>().SingleOrDefaultAsync(
                predicate: o => o.TransactionId == transactionId && o.Status == "Pending",
                    include: t => t.Include(tr => tr.TransactionDetails));

            if (transaction == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Transaction not found or is not in Pending status"
                };
            }

            transaction.Status = "Completed";
            transaction.UpdatedDate = DateTime.Now;

            var senderAccount = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: a => a.AccountId == senderAccountId);

            if (senderAccount == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Sender account not found"
                };
            }

            senderAccount.Point += transaction.TotalPoint;

            foreach (var detail in transaction.TransactionDetails)
            {
                var donation = await _unitOfWork.GetRepository<Donation>().SingleOrDefaultAsync(
                    predicate: d => d.DonationId == detail.DonationId);

                if (donation != null)
                {
                    donation.Status = "Claimed";
                    _unitOfWork.GetRepository<Donation>().UpdateAsync(donation);
                }
            }

            _unitOfWork.GetRepository<Transaction>().UpdateAsync(transaction);
            _unitOfWork.GetRepository<Account>().UpdateAsync(senderAccount);

            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult
            {
                Status = 1,
                Message = "Transaction completed, sender's points updated, and donation status changed to Claimed"
            };
        }
        #endregion

        #region Unused Transaction
        // Accept the transaction - Sender
        public async Task<IGiveandtakeResult> ChangeTransactionStatusToAccepted(int transactionId, int senderAccountId)
        {
            var transaction = await _unitOfWork.GetRepository<Transaction>()
                .SingleOrDefaultAsync(
                    predicate: t => t.TransactionId == transactionId &&
                                    t.TransactionDetails.Any(td => td.Donation.AccountId == senderAccountId),
                    include: t => t.Include(tr => tr.TransactionDetails));

            if (transaction == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Transaction not found or you are not authorized to change this transaction."
                };
            }

            if (transaction.Status == "Suspended")
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Cannot change status because the transaction is currently suspended."
                };
            }

            transaction.Status = "Accepted";
            transaction.UpdatedDate = DateTime.Now;
            _unitOfWork.GetRepository<Transaction>().UpdateAsync(transaction);

            foreach (var detail in transaction.TransactionDetails)
            {
                var donation = await _unitOfWork.GetRepository<Donation>().SingleOrDefaultAsync(
                    predicate: d => d.DonationId == detail.DonationId);

                if (donation != null)
                {
                    donation.Status = "Hiding";
                    _unitOfWork.GetRepository<Donation>().UpdateAsync(donation);
                }
            }

            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult
            {
                Status = 1,
                Message = "Transaction status changed to Accepted and related donation status changed to Hiding"
            };
        }

        // Reject the transaction - Sender
        public async Task<IGiveandtakeResult> ChangeTransactionStatusToRejected(int transactionId, int senderAccountId)
        {
            var transaction = await _unitOfWork.GetRepository<Transaction>().SingleOrDefaultAsync(
                predicate: t => t.TransactionId == transactionId &&
                                t.TransactionDetails.Any(td => td.Donation.AccountId == senderAccountId));

            if (transaction == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Transaction not found or you are not authorized to change this transaction."
                };
            }

            if (transaction.Status == "Suspended")
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Cannot change status because the transaction is currently suspended."
                };
            }

            transaction.Status = "Rejected";
            transaction.UpdatedDate = DateTime.Now;
            _unitOfWork.GetRepository<Transaction>().UpdateAsync(transaction);
            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult
            {
                Status = 1,
                Message = "Transaction status changed to Rejected"
            };
        }
        #endregion
    }
}
