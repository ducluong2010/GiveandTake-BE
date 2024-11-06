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

        public async Task<IGiveandtakeResult> CreateTradeRequest(TradeRequestDTO tradeRequestDTO)
        {
            GiveandtakeResult result = new GiveandtakeResult();

            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate:
                c => c.AccountId == tradeRequestDTO.AccountId);
            if (account == null)
            {
                return new GiveandtakeResult(-1, "Account not found");
            }

            // Check if the donation is valid for trading
            var requestDonation = await _unitOfWork.GetRepository<Donation>()
                .SingleOrDefaultAsync(predicate: d => d.DonationId == tradeRequestDTO.RequestDonationId && d.Type == 3 && d.Status == "Approved");

            if (requestDonation == null)
            {
                result.Status = -1;
                result.Message = "Request donation not found or not avaiable for trading";
                return result;
            }

            // Check if the requester is the onwer of the donation
            if (requestDonation.AccountId == tradeRequestDTO.AccountId)
            {
                return new GiveandtakeResult(-1, "You cannot request your own donation.");
            }

            // Check if the trade donation is valid
            var tradeDonation = await _unitOfWork.GetRepository<Donation>()
                .SingleOrDefaultAsync(predicate: d => d.DonationId == tradeRequestDTO.TradeDonationId &&
                                            (d.Type == 1 || d.Type == 3) &&
                                            d.AccountId == tradeRequestDTO.AccountId);

            if (tradeDonation == null)
            {
                result.Status = -1;
                result.Message = "Trade donation is not valid. Is it really belong to you?";
                return result;
            }

            // Check if the user has already requested this donation
            var existingUserTradeRequest = await _unitOfWork.GetRepository<TradeRequest>()
                .SingleOrDefaultAsync(predicate: x => x.AccountId == tradeRequestDTO.AccountId &&
                                                    x.TradeDonationId == tradeRequestDTO.TradeDonationId &&
                                                    x.RequestDonationId == tradeRequestDTO.RequestDonationId);
            if (existingUserTradeRequest != null)
            {
                return new GiveandtakeResult(-1, "You have already requested this donation.");
            }

            // Check if the donation is already involved in an active trade request
            var existingActiveTradeRequest = await _unitOfWork.GetRepository<TradeRequest>()
                .SingleOrDefaultAsync(predicate: x => x.TradeDonationId == tradeRequestDTO.TradeDonationId
                                                       && (x.Status == "Pending" || x.Status == "Accepted"));

            if (existingActiveTradeRequest != null)
            {
                return new GiveandtakeResult(-1, "This trade donation is already involved in an active trade request. Please wait until the current request is resolved.");
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
                result = new GiveandtakeResult(1, "Create Successful");
            }
            else
            {
                result.Status = -1;
                result.Message = "Create Unsuccessfully";
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
                return new GiveandtakeResult(-1, "Trade request not found.");
            }

            if (tradeRequest.AccountId != accountId)
            {
                return new GiveandtakeResult(-1, "You can only cancel your own trade requests.");
            }

            var invalidStatuses = new List<string> { "Cancelled", "Accepted" };
            if (invalidStatuses.Contains(tradeRequest.Status))
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = $"Request has been {tradeRequest.Status.ToLower()}"
                };
            }

            tradeRequest.Status = "Cancelled";

            _unitOfWork.GetRepository<TradeRequest>().UpdateAsync(tradeRequest);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccessful)
            {
                result.Status = -1;
                result.Message = "Cancel unsuccessfully.";
            }
            else
            {
                result.Status = 1;
                result.Message = "Trade request canceled successfully.";
            }

            return result;
        }

        public async Task<IGiveandtakeResult> DeleteTradeRequests()
        {
            GiveandtakeResult result = new GiveandtakeResult();

            var tradeRequests = await _unitOfWork.GetRepository<TradeRequest>()
                .GetListAsync(predicate: r => r.Status == "Rejected" || r.Status == "Cancelled");

            if (tradeRequests == null || !tradeRequests.Any())
            {
                return new GiveandtakeResult(-1, "No trade requests found with status 'Rejected' or 'Cancelled'.");
            }

            foreach (var request in tradeRequests)
            {
                _unitOfWork.GetRepository<TradeRequest>().DeleteAsync(request);
            }

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (isSuccessful)
            {
                result.Status = 1;
                result.Message = "Trade requests deleted successfully.";
            }
            else
            {
                result.Status = -1;
                result.Message = "Delete unsuccessfully.";
            }
            return result;
        }

        public async Task<IGiveandtakeResult> DeleteTradeRequest(int requestId)
        {
            GiveandtakeResult result = new GiveandtakeResult();

            var tradeRequest = await _unitOfWork.GetRepository<TradeRequest>()
                .SingleOrDefaultAsync(predicate: r => r.TradeRequestId == requestId && (r.Status == "Rejected" || r.Status == "Cancelled"));

            if (tradeRequest == null)
            {
                return new GiveandtakeResult(-1, "Trade request not found or not in status 'Rejected' or 'Cancelled'.");
            }

            _unitOfWork.GetRepository<TradeRequest>().DeleteAsync(tradeRequest);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (isSuccessful)
            {
                result.Status = 1;
                result.Message = "Trade request deleted successfully.";
            }
            else
            {
                result.Status = -1;
                result.Message = "Delete unsuccessfully.";
            }
            return result;
        }

    }
}
