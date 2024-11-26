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
            var transactionsList = await _unitOfWork.GetRepository<Transaction>()
                .GetListAsync(
                    selector: t => new
                    {
                        Transaction = new GetTransaction()
                        {
                            TransactionId = t.TransactionId,
                            TotalPoint = t.TotalPoint,
                            CreatedDate = t.CreatedDate,
                            UpdatedDate = t.UpdatedDate,
                            Status = t.Status,
                            AccountId = t.AccountId,
                            IsFeedback = t.IsFeedback
                        },
                        TransactionDetails = t.TransactionDetails.Select(td => new GetTransactionDetailDTO()
                        {
                            TransactionDetailId = td.TransactionDetailId,
                            TransactionId = td.TransactionId,
                            DonationId = td.DonationId,
                            Qrcode = td.Qrcode
                        }).ToList()
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
                            AccountId = o.AccountId,
                            IsFeedback = o.IsFeedback
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


        // Get transaction logged in user had received - Important
        public async Task<IGiveandtakeResult> GetTransactionsByAccount(int id)
        {
            var transactionsList = await _unitOfWork.GetRepository<Transaction>().GetListAsync(
                 predicate: o => o.AccountId == id && (o.Status == "Completed" || o.Status == "Pending"),
                 selector: o => new
                 {
                     Transaction = new GetTransaction()
                     {
                         TransactionId = o.TransactionId,
                         TotalPoint = o.TotalPoint,
                         CreatedDate = o.CreatedDate,
                         UpdatedDate = o.UpdatedDate,
                         Status = o.Status,
                         AccountId = o.AccountId,
                         IsFeedback = o.IsFeedback
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

        // Get transaction contained donation of logged in user - Important
        public async Task<IGiveandtakeResult> GetTransactionsByDonationForSender(int senderAccountId)
        {
            var transactionsList = await _unitOfWork.GetRepository<Transaction>()
                .GetListAsync(
                    predicate: t => (t.Status == "Pending" || t.Status == "Completed") && t.TransactionDetails.Any(td =>
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

        // Get transaction status
        public async Task<IGiveandtakeResult> GetTransactionStatus(int transactionId)
        {
            var transactionStatus = await _unitOfWork.GetRepository<Transaction>()
                .SingleOrDefaultAsync(
                    predicate: o => o.TransactionId == transactionId,
                    selector: o => new
                    {
                        TransactionId = o.TransactionId,
                        Status = o.Status
                    });

            if (transactionStatus == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Transaction not found"
                };
            }

            return new GiveandtakeResult(transactionStatus);
        }

        // Get completed transactions by (sender) account id
        public async Task<IGiveandtakeResult> GetCompletedTransactionsByAccountId(int senderAccountId)
        {
            var transactionsList = await _unitOfWork.GetRepository<Transaction>()
                .GetListAsync(
                    predicate: t => t.Status == "Completed" && t.TransactionDetails.Any(td =>
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
                            Qrcode = null 
                        }).ToList()
                    });

            if (transactionsList == null || !transactionsList.Any())
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "No completed transactions found"
                };
            }

            return new GiveandtakeResult(transactionsList);
        }


        #endregion

        #region Specific User Transaction

        // Create transaction and transaction detail at the same time - Sender
        public async Task<IGiveandtakeResult> CreateTransactionWithDetail(CreateTransaction createTransaction,
            TransactionDetailDTO transactionDetailDto, int senderAccountId)
        {
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

            donation.Status = "Hiding";
            _unitOfWork.GetRepository<Donation>().UpdateAsync(donation);

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

            Transaction transaction = new Transaction
            {
                TotalPoint = donation.Point,
                CreatedDate = createTransaction.CreatedDate,
                UpdatedDate = createTransaction.UpdatedDate,
                Status = "Pending",
                AccountId = createTransaction.AccountId
            };

            await _unitOfWork.GetRepository<Transaction>().InsertAsync(transaction);
            await _unitOfWork.CommitAsync();

            transactionDetailDto.TransactionId = transaction.TransactionId;
            TransactionDetail transactionDetail = new TransactionDetail
            {
                TransactionId = transaction.TransactionId,
                DonationId = transactionDetailDto.DonationId,
                Qrcode = null 
            };

            await _unitOfWork.GetRepository<TransactionDetail>().InsertAsync(transactionDetail);
            await _unitOfWork.CommitAsync(); 

            var qrCodeResult = await _transactionDetailBusiness.GenerateQRCode(transaction.TransactionId, transactionDetail.TransactionDetailId, (int)transactionDetailDto.DonationId);

            if (qrCodeResult.Status < 0)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = qrCodeResult.Message
                };
            }

            transactionDetail.Qrcode = qrCodeResult.Data.ToString();
            _unitOfWork.GetRepository<TransactionDetail>().UpdateAsync(transactionDetail);

            request.Status = "Accepted";
            _unitOfWork.GetRepository<Request>().UpdateAsync(request);

            var otherRequests = await _unitOfWork.GetRepository<Request>().GetListAsync(
                predicate: r => r.DonationId == donation.DonationId && r.RequestId != request.RequestId);

            foreach (var otherRequest in otherRequests)
            {
                otherRequest.Status = "Rejected";
                _unitOfWork.GetRepository<Request>().UpdateAsync(otherRequest);
            }

            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult
            {
                Status = 1,
                Message = "Transaction and TransactionDetail created successfully, QR Code generated, and other requests rejected.",
                Data = new
                {
                    TransactionId = transaction.TransactionId,
                    AccountId = transaction.AccountId,
                    DonationId = transactionDetail.DonationId,
                    Qrcode = transactionDetail.Qrcode 
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

            senderAccount.Point += senderAccount.IsPremium == true ? transaction.TotalPoint * 2 : transaction.TotalPoint;

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

        // Cancel the transaction - Sender
        public async Task<IGiveandtakeResult> CancelTransaction(int transactionId, int senderAccountId)
        {
            // Lấy thông tin transaction
            var transaction = await _unitOfWork.GetRepository<Transaction>().SingleOrDefaultAsync(
                predicate: t => t.TransactionId == transactionId);

            if (transaction == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Transaction not found."
                };
            }

            var daysSinceCreation = (DateTime.UtcNow - transaction.CreatedDate)?.TotalDays;
            if (daysSinceCreation.HasValue && daysSinceCreation < 5)
            {
                var daysRemaining = Math.Ceiling(5 - daysSinceCreation.Value);
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = $"Transaction can only be canceled after 5 days from its creation date. Please wait {daysRemaining} more day(s)."
                };
            }


            // Lấy thông tin donation liên quan
            var transactionDetail = await _unitOfWork.GetRepository<TransactionDetail>().SingleOrDefaultAsync(
                predicate: td => td.TransactionId == transaction.TransactionId);

            if (transactionDetail == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Transaction detail not found."
                };
            }

            var donation = await _unitOfWork.GetRepository<Donation>().SingleOrDefaultAsync(
                predicate: d => d.DonationId == transactionDetail.DonationId && d.AccountId == senderAccountId);

            if (donation == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Donation not found or it does not belong to you."
                };
            }

            if (transaction.Status == "Completed")
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Transaction cannot be cancelled after it has been completed."
                };
            }

            if (transaction.Status == "Cancelled")
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Transaction has already been cancelled."
                };
            }

            // Cập nhật trạng thái transaction
            transaction.Status = "Cancelled";
            _unitOfWork.GetRepository<Transaction>().UpdateAsync(transaction);

            // Cập nhật trạng thái donation
            donation.Status = "Approved";
            donation.Type = 1; // Quay lại kho
            _unitOfWork.GetRepository<Donation>().UpdateAsync(donation);

            // Cập nhật trạng thái request liên quan
            var request = await _unitOfWork.GetRepository<Request>().SingleOrDefaultAsync(
                predicate: r => r.DonationId == donation.DonationId && r.Status == "Accepted");

            if (request != null)
            {
                request.Status = "Cancelled";
                _unitOfWork.GetRepository<Request>().UpdateAsync(request);
            }

            // Commit các thay đổi
            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult
            {
                Status = 1,
                Message = "Transaction cancelled successfully, and donation status updated to 'Approved'."
            };
        }

        // Toggle feedback status of a transaction
        public async Task<IGiveandtakeResult> ToggleIsFeedbackStatus(int transactionId)
        {
            var existingTransaction = await _unitOfWork.GetRepository<Transaction>()
                .SingleOrDefaultAsync(predicate: t => t.TransactionId == transactionId);

            if (existingTransaction == null)
            {
                return new GiveandtakeResult(-1, "Transaction not found");
            }

            existingTransaction.IsFeedback = !existingTransaction.IsFeedback;
            _unitOfWork.GetRepository<Transaction>().UpdateAsync(existingTransaction);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful
                ? new GiveandtakeResult(1, "Feedback status updated successfully")
                : new GiveandtakeResult(-1, "Update unsuccessfully");
        }
        #endregion
    }
}
