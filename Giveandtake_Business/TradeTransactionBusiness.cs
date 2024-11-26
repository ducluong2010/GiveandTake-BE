using GiveandTake_Repo.DTOs.TradeTransaction;
using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GiveandTake_Repo.DTOs.TradeTransaction.TradeTransactionDTO;
using static GiveandTake_Repo.DTOs.Transaction.TransactionDTO;
using TradeTransaction = GiveandTake_Repo.Models.TradeTransaction;

namespace Giveandtake_Business
{
    public class TradeTransactionBusiness
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly TradeTransactionDetailBusiness _tradeTransactionDetailBusiness;

        public TradeTransactionBusiness()
        {
            _unitOfWork = new UnitOfWork();
            _tradeTransactionDetailBusiness = new TradeTransactionDetailBusiness();
        }

        #region Basic Trade Transaction

        // Get all trade transactions
        public async Task<IGiveandtakeResult> GetAllTradeTransaction()
        {
            var tradeList = await _unitOfWork.GetRepository<TradeTransaction>()
                .GetListAsync(selector: o => new
                {
                    TradeTransaction = new GetTradeTransaction()
                    {
                        TradeTransactionId = o.TradeTransactionId,
                        AccountId = o.AccountId,
                        TradeDonationId = o.TradeDonationId,
                        CreatedDate = o.CreatedDate,
                        UpdatedDate = o.UpdatedDate,
                        Status = o.Status
                    },
                    TradeTransactionDetails = o.TradeTransactionDetails.Select(td => new GetTradeTransactionDetailDTO()
                    {
                        TradeTransactionDetailId = td.TradeTransactionDetailId,
                        TradeTransactionId = td.TradeTransactionId,
                        RequestDonationId = td.RequestDonationId,
                        Qrcode = td.Qrcode
                    }).ToList()
                });
            return new GiveandtakeResult(tradeList);
        }

        // Get trade transaction by id
        public async Task<IGiveandtakeResult> GetTradeTransactionById(int id)
        {
            var tradeTransaction = await _unitOfWork.GetRepository<TradeTransaction>()
                .SingleOrDefaultAsync
                (predicate: o => o.TradeTransactionId == id,
                selector: o => new
                {
                    TradeTransaction = new GetTradeTransaction()
                    {
                        TradeTransactionId = o.TradeTransactionId,
                        AccountId = o.AccountId,
                        TradeDonationId = o.TradeDonationId,
                        CreatedDate = o.CreatedDate,
                        UpdatedDate = o.UpdatedDate,
                        Status = o.Status
                    },
                    TradeTransactionDetails = o.TradeTransactionDetails.Select(td => new GetTradeTransactionDetailDTO()
                    {
                        TradeTransactionDetailId = td.TradeTransactionDetailId,
                        TradeTransactionId = td.TradeTransactionId,
                        RequestDonationId = td.RequestDonationId,
                        Qrcode = td.Qrcode
                    }).ToList()
                });

            if (tradeTransaction == null)
            {
                return new GiveandtakeResult(-1, "Trade transaction not found");
            }

            return new GiveandtakeResult(new
            {
                TradeTransaction = tradeTransaction.TradeTransaction,
                TradeTransactionDetails = tradeTransaction.TradeTransactionDetails
            });
        }

        // Get trade transaction by account id
        public async Task<IGiveandtakeResult> GetTradeTransactionByAccountId(int accountId)
        {
            var tradeList = await _unitOfWork.GetRepository<TradeTransaction>()
                .GetListAsync
                (predicate: o => o.AccountId == accountId && (o.Status == "Completed" || o.Status == "Pending"),
                selector: o => new
                {
                    TradeTransaction = new GetTradeTransaction()
                    {
                        TradeTransactionId = o.TradeTransactionId,
                        AccountId = o.AccountId,
                        TradeDonationId = o.TradeDonationId,
                        CreatedDate = o.CreatedDate,
                        UpdatedDate = o.UpdatedDate,
                        Status = o.Status
                    },
                    TradeTransactionDetails = o.TradeTransactionDetails.Select(td => new GetTradeTransactionDetailDTO()
                    {
                        TradeTransactionDetailId = td.TradeTransactionDetailId,
                        TradeTransactionId = td.TradeTransactionId,
                        RequestDonationId = td.RequestDonationId,
                        Qrcode = td.Qrcode
                    }).ToList()
                });

            return new GiveandtakeResult(tradeList);
        }

        // Get trade transaction status
        public async Task<IGiveandtakeResult> GetTradeTransactionStatus(int id)
        {
            var tradeTransactionStatus = await _unitOfWork.GetRepository<TradeTransaction>()
                .SingleOrDefaultAsync
                (predicate: o => o.TradeTransactionId == id,
                selector: o => new
                {
                        TradeTransactionId = o.TradeTransactionId,
                        Status = o.Status      
                });

            if (tradeTransactionStatus == null)
            {
                return new GiveandtakeResult(-1, "Trade transaction not found");
            }

            return new GiveandtakeResult(tradeTransactionStatus);
        }

        // Accept request, create transaction
        public async Task<IGiveandtakeResult> AcceptTradeRequest(int tradeRequestId, int loggedInAccountId)
        {
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: a => a.AccountId == loggedInAccountId);
            if (account == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Account not found"
                };
            }

            var tradeRequest = await _unitOfWork.GetRepository<TradeRequest>().SingleOrDefaultAsync(
                predicate: tr => tr.TradeRequestId == tradeRequestId);
            if (tradeRequest == null || tradeRequest.Status != "Pending")
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Trade request not found or already processed."
                };
            }

            var requestDonation = await _unitOfWork.GetRepository<Donation>().SingleOrDefaultAsync(
                predicate: d => d.DonationId == tradeRequest.RequestDonationId && d.Status == "Approved" && d.AccountId == loggedInAccountId);
            if (requestDonation == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Request donation not found, not approved, or does not belong to the logged-in user."
                };
            }

            var tradeDonation = await _unitOfWork.GetRepository<Donation>().SingleOrDefaultAsync(
                predicate: d => d.DonationId == tradeRequest.TradeDonationId && d.Status == "Approved");
            if (tradeDonation == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Trade donation not found or not approved."
                };
            }

            requestDonation.Status = "Hiding";
            tradeDonation.Status = "Hiding";
            _unitOfWork.GetRepository<Donation>().UpdateAsync(requestDonation);
            _unitOfWork.GetRepository<Donation>().UpdateAsync(tradeDonation);

            var tradeTransaction = new TradeTransaction
            {
                AccountId = tradeRequest.AccountId,
                TradeDonationId = tradeDonation.DonationId,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                Status = "Pending"
            };
            await _unitOfWork.GetRepository<TradeTransaction>().InsertAsync(tradeTransaction);
            await _unitOfWork.CommitAsync(); 

            var tradeTransactionDetail = new TradeTransactionDetail
            {
                TradeTransactionId = tradeTransaction.TradeTransactionId,
                RequestDonationId = requestDonation.DonationId,
                Qrcode = null 
            };
            await _unitOfWork.GetRepository<TradeTransactionDetail>().InsertAsync(tradeTransactionDetail);
            await _unitOfWork.CommitAsync(); 

            var qrCodeResult = await _tradeTransactionDetailBusiness.GenerateQRCode(
                tradeTransaction.TradeTransactionId,
                tradeTransactionDetail.TradeTransactionDetailId,
                (int)tradeTransactionDetail.RequestDonationId);

            if (qrCodeResult.Status < 0)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = qrCodeResult.Message
                };
            }

            tradeTransactionDetail.Qrcode = qrCodeResult.Data.ToString();
            _unitOfWork.GetRepository<TradeTransactionDetail>().UpdateAsync(tradeTransactionDetail);

            tradeRequest.Status = "Accepted";
            _unitOfWork.GetRepository<TradeRequest>().UpdateAsync(tradeRequest);

            var otherRequests = await _unitOfWork.GetRepository<TradeRequest>().GetListAsync(
                predicate: tr => tr.RequestDonationId == requestDonation.DonationId && tr.TradeRequestId != tradeRequest.TradeRequestId);

            foreach (var otherRequest in otherRequests)
            {
                otherRequest.Status = "Rejected";
                _unitOfWork.GetRepository<TradeRequest>().UpdateAsync(otherRequest);
            }

            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult
            {
                Status = 1,
                Message = "Trade transaction and detail created successfully, QR code generated, and other requests rejected.",
                Data = new
                {
                    TradeTransactionId = tradeTransaction.TradeTransactionId,
                    AccountId = tradeTransaction.AccountId,
                    RequestDonationId = tradeTransactionDetail.RequestDonationId,
                    Qrcode = tradeTransactionDetail.Qrcode
                }
            };
        }

        // Reject request
        public async Task<IGiveandtakeResult> RejectTradeRequest(int tradeRequestId, int loggedInAccountId)
        {
            var tradeRequest = await _unitOfWork.GetRepository<TradeRequest>()
                .SingleOrDefaultAsync(predicate: tr => tr.TradeRequestId == tradeRequestId);

            if (tradeRequest == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Trade request not found"
                };
            }

            var requestDonation = await _unitOfWork.GetRepository<Donation>()
                .SingleOrDefaultAsync(predicate: d => d.DonationId == tradeRequest.RequestDonationId && d.AccountId == loggedInAccountId);

            if (requestDonation == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "You do not have permission to reject this trade request"
                };
            }

            if (tradeRequest.Status == "Accepted")
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Trade request has already been accepted and cannot be rejected"
                };
            }

            tradeRequest.Status = "Rejected";
            _unitOfWork.GetRepository<TradeRequest>().UpdateAsync(tradeRequest);

            bool status = await _unitOfWork.CommitAsync() > 0;
            if (status)
            {
                return new GiveandtakeResult
                {
                    Status = 1,
                    Message = "Trade request rejected successfully"
                };
            }
            else
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Failed to reject trade request"
                };
            }
        }

        // Complete trade transaction
        public async Task<IGiveandtakeResult> CompleteTradeTransaction(int tradeTransactionId, int loggedInAccountId)
        {
            var tradeTransaction = await _unitOfWork.GetRepository<TradeTransaction>().SingleOrDefaultAsync(
                predicate: tt => tt.TradeTransactionId == tradeTransactionId);

            if (tradeTransaction == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Trade transaction not found."
                };
            }

            var tradeTransactionDetail = await _unitOfWork.GetRepository<TradeTransactionDetail>()
                .SingleOrDefaultAsync(
                    predicate: ttd => ttd.TradeTransactionId == tradeTransactionId,
                    include: ttd => ttd.Include(d => d.RequestDonation));

            if (tradeTransactionDetail == null || tradeTransactionDetail.RequestDonation == null || tradeTransactionDetail.RequestDonation.AccountId != loggedInAccountId)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "You do not have permission to complete this trade transaction."
                };
            }


            var requestDonation = await _unitOfWork.GetRepository<Donation>().SingleOrDefaultAsync(
                predicate: d => d.DonationId == tradeTransactionDetail.RequestDonationId);

            var tradeDonation = await _unitOfWork.GetRepository<Donation>().SingleOrDefaultAsync(
                predicate: d => d.DonationId == tradeTransaction.TradeDonationId);

            if (requestDonation == null || tradeDonation == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Donations involved in the trade transaction not found."
                };
            }

            var tempAccountId = requestDonation.AccountId;
            requestDonation.AccountId = tradeDonation.AccountId;
            tradeDonation.AccountId = tempAccountId;

            requestDonation.Type = 1;
            tradeDonation.Type = 1;

            requestDonation.Status = "Approved";
            tradeDonation.Status = "Approved";


            _unitOfWork.GetRepository<Donation>().UpdateAsync(requestDonation);
            _unitOfWork.GetRepository<Donation>().UpdateAsync(tradeDonation);

            tradeTransaction.Status = "Completed";
            tradeTransaction.UpdatedDate = DateTime.UtcNow;
            _unitOfWork.GetRepository<TradeTransaction>().UpdateAsync(tradeTransaction);

            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult
            {
                Status = 1,
                Message = "Trade transaction completed successfully.",
                Data = new
                {
                    TradeTransactionId = tradeTransaction.TradeTransactionId,
                    Status = tradeTransaction.Status,
                    RequestDonationOwnerId = requestDonation.AccountId,
                    TradeDonationOwnerId = tradeDonation.AccountId
                }
            };
        }

        // Cancel trade transaction
        public async Task<IGiveandtakeResult> CancelTradeTransaction(int tradeTransactionId, int loggedInAccountId)
        {
            var tradeTransaction = await _unitOfWork.GetRepository<TradeTransaction>().SingleOrDefaultAsync(
                predicate: tt => tt.TradeTransactionId == tradeTransactionId);

            if (tradeTransaction == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Trade transaction not found."
                };
            }

            var tradeTransactionDetail = await _unitOfWork.GetRepository<TradeTransactionDetail>().SingleOrDefaultAsync(
                predicate: ttd => ttd.TradeTransactionId == tradeTransactionId,
                include: ttd => ttd.Include(d => d.RequestDonation));

            if (tradeTransactionDetail == null || tradeTransactionDetail.RequestDonation == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Trade transaction detail or request donation not found."
                };
            }

            // Kiểm tra quyền sở hữu - chỉ người sở hữu RequestDonation hoặc TradeDonation mới có thể hủy
            var requestDonation = tradeTransactionDetail.RequestDonation;
            var tradeDonation = await _unitOfWork.GetRepository<Donation>().SingleOrDefaultAsync(
                predicate: d => d.DonationId == tradeTransaction.TradeDonationId);

            if (tradeDonation == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Trade donation not found."
                };
            }

            // Kiểm tra nếu loggedInAccountId không phải chủ sở hữu của cả hai món đồ
            if (requestDonation.AccountId != loggedInAccountId && tradeDonation.AccountId != loggedInAccountId)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "You do not have permission to cancel this trade transaction."
                };
            }


            if (tradeTransaction.Status == "Completed")
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Trade transaction has already been completed and cannot be canceled."
                };
            }

            if (tradeTransaction.Status == "Cancelled")
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Trade transaction has already been canceled."
                };
            }

            var daysSinceCreation = (DateTime.UtcNow - tradeTransaction.CreatedDate)?.TotalDays;
            if (daysSinceCreation.HasValue && daysSinceCreation < 5)
            {
                var daysRemaining = Math.Ceiling(5 - daysSinceCreation.Value);
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = $"Trade transaction can only be canceled after 5 days from its creation date. Please wait {daysRemaining} more day(s)."
                };
            }

            tradeDonation.Type = 1; 
            requestDonation.Type = 1; 


            tradeDonation.Status = "Approved"; 
            requestDonation.Status = "Approved";

            _unitOfWork.GetRepository<Donation>().UpdateAsync(requestDonation);
            _unitOfWork.GetRepository<Donation>().UpdateAsync(tradeDonation);

            tradeTransaction.Status = "Cancelled";
            tradeTransaction.UpdatedDate = DateTime.UtcNow;
            _unitOfWork.GetRepository<TradeTransaction>().UpdateAsync(tradeTransaction);

            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult
            {
                Status = 1,
                Message = "Trade transaction canceled successfully.",
            };
        }

        // Get pending trade transaction contained donation of logged in user
        public async Task<IGiveandtakeResult> GetTradeTransactionByDonationForSender(int senderAccountId)
        {
            var tradeList = await _unitOfWork.GetRepository<TradeTransaction>()
                .GetListAsync
                (predicate: t => (t.Status == "Pending") && t.TradeTransactionDetails.Any(td =>
                        td.RequestDonationId != null && td.RequestDonation.AccountId == senderAccountId),
                selector: o => new
                {
                    TradeTransaction = new GetTradeTransaction()
                    {
                        TradeTransactionId = o.TradeTransactionId,
                        AccountId = o.AccountId,
                        TradeDonationId = o.TradeDonationId,
                        CreatedDate = o.CreatedDate,
                        UpdatedDate = o.UpdatedDate,
                        Status = o.Status
                    },
                    TradeTransactionDetails = o.TradeTransactionDetails.Select(td => new GetTradeTransactionDetailDTO()
                    {
                        TradeTransactionDetailId = td.TradeTransactionDetailId,
                        TradeTransactionId = td.TradeTransactionId,
                        RequestDonationId = td.RequestDonationId,
                        Qrcode = td.Qrcode
                    }).ToList()
                });

            if (tradeList == null || !tradeList.Any())
            {
                return new GiveandtakeResult(-1, "Trade transaction not found");
            }

            return new GiveandtakeResult(tradeList);
        }

            #endregion
     }
}
