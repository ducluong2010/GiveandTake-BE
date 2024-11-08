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
                (predicate: o => o.AccountId == accountId,
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
            // Kiểm tra tài khoản
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

            // Kiểm tra yêu cầu trao đổi
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

            // Kiểm tra request donation
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

            // Kiểm tra trade donation
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

            // Cập nhật trạng thái donations thành "Hiding"
            requestDonation.Status = "Hiding";
            tradeDonation.Status = "Hiding";
            _unitOfWork.GetRepository<Donation>().UpdateAsync(requestDonation);
            _unitOfWork.GetRepository<Donation>().UpdateAsync(tradeDonation);

            // Tạo trade transaction
            var tradeTransaction = new TradeTransaction
            {
                AccountId = tradeRequest.AccountId,
                TradeDonationId = tradeDonation.DonationId,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                Status = "Pending"
            };
            await _unitOfWork.GetRepository<TradeTransaction>().InsertAsync(tradeTransaction);
            await _unitOfWork.CommitAsync(); // Commit để có TradeTransactionId

            // Tạo trade transaction detail
            var tradeTransactionDetail = new TradeTransactionDetail
            {
                TradeTransactionId = tradeTransaction.TradeTransactionId,
                RequestDonationId = requestDonation.DonationId,
                Qrcode = null // QR code sẽ tạo sau
            };
            await _unitOfWork.GetRepository<TradeTransactionDetail>().InsertAsync(tradeTransactionDetail);
            await _unitOfWork.CommitAsync(); // Commit để có TradeTransactionDetailId

            // Tạo QR code và cập nhật trade transaction detail
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

            // Cập nhật trạng thái trade request thành "Accepted"
            tradeRequest.Status = "Accepted";
            _unitOfWork.GetRepository<TradeRequest>().UpdateAsync(tradeRequest);

            // Từ chối các yêu cầu khác cho cùng một requestDonationId
            var otherRequests = await _unitOfWork.GetRepository<TradeRequest>().GetListAsync(
                predicate: tr => tr.RequestDonationId == requestDonation.DonationId && tr.TradeRequestId != tradeRequest.TradeRequestId);

            foreach (var otherRequest in otherRequests)
            {
                otherRequest.Status = "Rejected";
                _unitOfWork.GetRepository<TradeRequest>().UpdateAsync(otherRequest);
            }

            await _unitOfWork.CommitAsync();

            // Trả về kết quả thành công
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
            // Lấy trade request từ database
            var tradeRequest = await _unitOfWork.GetRepository<TradeRequest>()
                .SingleOrDefaultAsync(predicate: tr => tr.TradeRequestId == tradeRequestId);

            // Kiểm tra nếu trade request không tồn tại
            if (tradeRequest == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Trade request not found"
                };
            }

            // Kiểm tra quyền sở hữu của account đối với request donation
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

            // Kiểm tra nếu trade request đã được chấp nhận thì không thể từ chối
            if (tradeRequest.Status == "Accepted")
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Trade request has already been accepted and cannot be rejected"
                };
            }

            // Cập nhật trạng thái trade request thành "Rejected"
            tradeRequest.Status = "Rejected";
            _unitOfWork.GetRepository<TradeRequest>().UpdateAsync(tradeRequest);

            // Commit thay đổi vào database
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
            // Kiểm tra tồn tại của trade transaction"
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

            // Kiểm tra quyền sở hữu: người thực hiện phải là người đã accept (sở hữu request donation id)
            var tradeTransactionDetail = await _unitOfWork.GetRepository<TradeTransactionDetail>()
                .SingleOrDefaultAsync(
                    predicate: ttd => ttd.TradeTransactionId == tradeTransactionId,
                    include: ttd => ttd.Include(d => d.RequestDonation)); // Include để lấy dữ liệu request donation

            if (tradeTransactionDetail == null || tradeTransactionDetail.RequestDonation == null || tradeTransactionDetail.RequestDonation.AccountId != loggedInAccountId)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "You do not have permission to complete this trade transaction."
                };
            }


            // Lấy requestDonation và tradeDonation
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

            // Đổi chủ sở hữu cho requestDonation và tradeDonation, set type về 1 (nằm trong kho)
            var tempAccountId = requestDonation.AccountId;
            requestDonation.AccountId = tradeDonation.AccountId;
            tradeDonation.AccountId = tempAccountId;

            requestDonation.Type = 1;
            tradeDonation.Type = 1;

            _unitOfWork.GetRepository<Donation>().UpdateAsync(requestDonation);
            _unitOfWork.GetRepository<Donation>().UpdateAsync(tradeDonation);

            // Cập nhật trạng thái trade transaction thành "Completed"
            tradeTransaction.Status = "Completed";
            tradeTransaction.UpdatedDate = DateTime.UtcNow;
            _unitOfWork.GetRepository<TradeTransaction>().UpdateAsync(tradeTransaction);

            // Lưu thay đổi
            await _unitOfWork.CommitAsync();

            // Trả về kết quả thành công
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


        #endregion
    }
}
