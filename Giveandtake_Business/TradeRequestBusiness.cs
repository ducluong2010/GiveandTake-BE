using Azure.Core;
using GiveandTake_Repo.DTOs.Request;
using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Business
{
    public class TradeRequestBusiness
    {
        private UnitOfWork _unitOfWork;
        public TradeRequestBusiness()
        {
            _unitOfWork = new UnitOfWork();
        }

        public async Task<IGiveandtakeResult> GetAllTradeRequests()
        {
            var tradeRequestList = await _unitOfWork.GetRepository<TradeRequest>()
                .GetListAsync(selector: x => new GetTradeRequestDTO
                {
                    TradeRequestId = x.TradeRequestId,
                    AccountId = x.AccountId,
                    TradeDonationId = x.TradeDonationId,
                    RequestDonationId = x.RequestDonationId,
                    RequestDate = x.RequestDate,
                    Status = x.Status
                });
            return new GiveandtakeResult(tradeRequestList);
        }

        public async Task<IGiveandtakeResult> GetTradeRequestById(int id)
        {
            var tradeRequest = await _unitOfWork.GetRepository<TradeRequest>()
                .SingleOrDefaultAsync(predicate: x => x.TradeRequestId == id, selector: x => new GetTradeRequestDTO
                {
                    TradeRequestId = x.TradeRequestId,
                    AccountId = x.AccountId,
                    TradeDonationId = x.TradeDonationId,
                    RequestDonationId = x.RequestDonationId,
                    RequestDate = x.RequestDate,
                    Status = x.Status
                });
            return new GiveandtakeResult(tradeRequest);
        }

        public async Task<IGiveandtakeResult> GetTradeRequestByAccountId(int accountId)
        {
            var tradeRequestList = await _unitOfWork.GetRepository<TradeRequest>()
                .GetListAsync(predicate: x => x.AccountId == accountId, selector: x => new GetTradeRequestDTO
                {
                    TradeRequestId = x.TradeRequestId,
                    AccountId = x.AccountId,
                    TradeDonationId = x.TradeDonationId,
                    RequestDonationId = x.RequestDonationId,
                    RequestDate = x.RequestDate,
                    Status = x.Status
                });
            return new GiveandtakeResult(tradeRequestList);
        }

        public async Task<IGiveandtakeResult> GetTradeRequestByTradeDonationId(int requestDonationId)
        {
            var tradeRequests = await _unitOfWork.GetRepository<TradeRequest>()
                .GetListAsync(
                    predicate: x => x.RequestDonationId == requestDonationId ,
                    selector: x => new
                    {
                        TradeRequest = new GetTradeRequestDTO
                        {
                            TradeRequestId = x.TradeRequestId,
                            AccountId = x.AccountId,
                            TradeDonationId = x.TradeDonationId,
                            RequestDonationId = x.RequestDonationId,
                            RequestDate = x.RequestDate,
                            Status = x.Status
                        },
                        IsPremium = x.Account.IsPremium
                    });

            var sortedTradeRequests = tradeRequests
                .OrderByDescending(x => x.IsPremium)
                .ThenBy(x => x.TradeRequest.RequestDate)
                .Select(x => x.TradeRequest)
                .ToList();

            return new GiveandtakeResult(sortedTradeRequests);
        }

        public async Task<IGiveandtakeResult> CreateTradeRequest(TradeRequestDTO tradeRequestDTO)
        {
            GiveandtakeResult result = new GiveandtakeResult();

            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate:
                c => c.AccountId == tradeRequestDTO.AccountId);
            if (account == null)
            {
                return new GiveandtakeResult(-1, "Không tìm thấy account");
            }

            // Check if the donation is valid for trading
            var requestDonation = await _unitOfWork.GetRepository<Donation>()
                .SingleOrDefaultAsync(predicate: d => d.DonationId == tradeRequestDTO.RequestDonationId && d.Type == 3 && d.Status == "Approved");

            if (requestDonation == null)
            {
                result.Status = -1;
                result.Message = "Món đồ được yêu cầu trao đổi không khả dụng";
                return result;
            }

            // Check if the requester is the onwer of the donation
            if (requestDonation.AccountId == tradeRequestDTO.AccountId)
            {
                return new GiveandtakeResult(-1, "Bạn không thể yêu cầu trao đổi chính món đồ của mình.");
            }

            // Check if the trade donation is valid
            var tradeDonation = await _unitOfWork.GetRepository<Donation>()
                .SingleOrDefaultAsync(predicate: d => d.DonationId == tradeRequestDTO.TradeDonationId &&
                                            (d.Type == 1 || d.Type == 3) &&
                                            d.AccountId == tradeRequestDTO.AccountId);

            if (tradeDonation == null)
            {
                result.Status = -1;
                result.Message = "Món đồ trao đổi không khả dụng. Hãy đăng nhập bằng chủ sở hữu.";
                return result;
            }

            // Check if the user has already requested this donation
            var existingUserTradeRequest = await _unitOfWork.GetRepository<TradeRequest>()
                .SingleOrDefaultAsync(predicate: x => x.AccountId == tradeRequestDTO.AccountId &&
                                                    x.TradeDonationId == tradeRequestDTO.TradeDonationId &&
                                                    x.RequestDonationId == tradeRequestDTO.RequestDonationId &&
                                                    x.Status == "Pending");
            if (existingUserTradeRequest != null)
            {
                return new GiveandtakeResult(-1, "Bạn đã yêu cầu trao đổi món đồ này rồi.");
            }

            // Check if the donation is already involved in an active trade request
            var existingActiveTradeRequest = await _unitOfWork.GetRepository<TradeRequest>()
                .SingleOrDefaultAsync(predicate: x => x.TradeDonationId == tradeRequestDTO.TradeDonationId
                                                       && (x.Status == "Pending" || x.Status == "Accepted"));

            if (existingActiveTradeRequest != null)
            {
                return new GiveandtakeResult(-1, "Món đồ này hiện đang được trao đổi rồi.");
            }


            // Create new trade request
            TradeRequest newTradeRequest = new TradeRequest
            {
                AccountId = tradeRequestDTO.AccountId,
                TradeDonationId = tradeRequestDTO.TradeDonationId,
                RequestDonationId = tradeRequestDTO.RequestDonationId,
                RequestDate = DateTime.Now,
                Status = "Pending"
            };

            await _unitOfWork.GetRepository<TradeRequest>().InsertAsync(newTradeRequest);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (isSuccessful)
            {
                result = new GiveandtakeResult(1, "Tạo yêu cầu trao đổi thành công");
            }
            else
            {
                result.Status = -1;
                result.Message = "Tạo yêu cầu trao đổi thất bại";
            }
            return result;
        }

        public async Task<IGiveandtakeResult> CancelTradeRequest(int requestId, int accountId)
        {
            var result = new GiveandtakeResult();

            var tradeRequest = await _unitOfWork.GetRepository<TradeRequest>()
                .SingleOrDefaultAsync(predicate: x => x.TradeRequestId == requestId);

            if (tradeRequest == null)
            {
                return new GiveandtakeResult(-1, "Không thể tìm thấy yêu cầu trao đổi.");
            }

            if (tradeRequest.AccountId != accountId)
            {
                return new GiveandtakeResult(-1, "Bạn chỉ có thể huỷ chính yêu cầu trao đổi của mình.");
            }

            var invalidStatuses = new List<string> { "Cancelled", "Accepted" };
            if (invalidStatuses.Contains(tradeRequest.Status))
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = $"Yêu cầu đã được {tradeRequest.Status.ToLower()} rồi!"
                };
            }

            tradeRequest.Status = "Cancelled";

            _unitOfWork.GetRepository<TradeRequest>().UpdateAsync(tradeRequest);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccessful)
            {
                result.Status = -1;
                result.Message = "Huỷ yêu cầu trao đổi thất bại.";
            }
            else
            {
                result.Status = 1;
                result.Message = "Huỷ yêu cầu trao đổi thành công.";
            }

            return result;
        }

        public async Task<IGiveandtakeResult> DeleteTradeRequest(int requestId)
        {
            GiveandtakeResult result = new GiveandtakeResult();

            var tradeRequest = await _unitOfWork.GetRepository<TradeRequest>()
                .SingleOrDefaultAsync(predicate: r => r.TradeRequestId == requestId);

            if (tradeRequest == null)
            {
                return new GiveandtakeResult(-1, "Không tìm thấy yêu cầu trao đổi.");
            }

            if (tradeRequest.Status == "Accepted")
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Yêu cầu đã được chủ món đồ chấp nhận, không thể xóa."
                };
            }

            _unitOfWork.GetRepository<TradeRequest>().DeleteAsync(tradeRequest);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (isSuccessful)
            {
                result.Status = 1;
                result.Message = "Yêu cầu trao đổi đã được xoá thành công.";
            }
            else
            {
                result.Status = -1;
                result.Message = "Xoá thất bại.";
            }
            return result;
        }

    }
}
