using GiveandTake_Repo.DTOs.Request;
using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Business
{
    public class RequestBusiness
    {
        private UnitOfWork _unitOfWork;
        public RequestBusiness()
        {
            _unitOfWork = new UnitOfWork();
        }

        #region Request
        // Get all request
        public async Task<IGiveandtakeResult> GetAllRequests()
        {
            var requestList = await _unitOfWork.GetRepository<Request>()
                .GetListAsync(selector: x => new GetRequestDTO
                {
                    RequestId = x.RequestId,
                    AccountId = x.AccountId,
                    DonationId = x.DonationId,
                    RequestDate = x.RequestDate,
                    Status = x.Status
                });
            return new GiveandtakeResult(requestList);
        }

        // Get request by id
        public async Task<IGiveandtakeResult> GetRequestById(int requestId)
        {
            var request = await _unitOfWork.GetRepository<Request>()
                .SingleOrDefaultAsync(predicate: c => c.RequestId == requestId,
                                      selector: x => new GetRequestDTO
                                      {
                                          RequestId = x.RequestId,
                                          AccountId = x.AccountId,
                                          DonationId = x.DonationId,
                                          RequestDate = x.RequestDate,
                                          Status = x.Status
                                      });
            return new GiveandtakeResult(request);
        }

        // Get request by donation id
        public async Task<IGiveandtakeResult> GetRequestByDonationId(int donationId)
        {
            var requestList = await _unitOfWork.GetRepository<Request>()
                .GetListAsync(predicate: c => c.DonationId == donationId,
                              selector: x => new GetRequestDTO
                              {
                                  RequestId = x.RequestId,
                                  AccountId = x.AccountId,
                                  DonationId = x.DonationId,
                                  RequestDate = x.RequestDate,
                                  Status = x.Status
                              });
            return new GiveandtakeResult(requestList);
        }

        // Get request by account id
        public async Task<IGiveandtakeResult> GetRequestByAccountId(int accountId)
        {
            var requestList = await _unitOfWork.GetRepository<Request>()
                .GetListAsync(predicate: c => c.AccountId == accountId,
                              selector: x => new GetRequestDTO
                              {
                                  RequestId = x.RequestId,
                                  AccountId = x.AccountId,
                                  DonationId = x.DonationId,
                                  RequestDate = x.RequestDate,
                                  Status = x.Status
                              });
            return new GiveandtakeResult(requestList);
        }

        // Create request
        public async Task<IGiveandtakeResult> CreateRequest(RequestDTO requestDTO)
        {
            GiveandtakeResult result = new GiveandtakeResult();

            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate:
                c => c.AccountId == requestDTO.DonationId);
            if (account == null)
            {
                return new GiveandtakeResult(-1, "Account not found");
            }

            var donation = await _unitOfWork.GetRepository<Donation>().SingleOrDefaultAsync(predicate:
                d => d.DonationId == requestDTO.DonationId);
            if (donation == null)
            {
                return new GiveandtakeResult(-1, "Donation not found");
            }

            Request request = new Request
            {
                AccountId = requestDTO.AccountId,
                DonationId = requestDTO.DonationId,
                RequestDate = DateTime.Now,
                Status = "Pending"
            };
            await _unitOfWork.GetRepository<Request>().InsertAsync(request);
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

        // Cancel request
        public async Task<IGiveandtakeResult> CancelRequest(int requestId, int receiverId)
        {
            GiveandtakeResult result = new GiveandtakeResult();

            var request = await _unitOfWork.GetRepository<Request>().SingleOrDefaultAsync(
                predicate: c => c.RequestId == requestId &&
                                c.AccountId == receiverId);

            if (request == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Request not found or you are not authorized to change this request"
                };
            }

            if (request.Status == "Cancelled")
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Request has been cancelled"
                };
            }

            if (request.Status == "Accepted")
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Request has been accepted"
                };
            }

            request.Status = "Cancelled";
            _unitOfWork.GetRepository<Request>().UpdateAsync(request);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (isSuccessful)
            {
                result = new GiveandtakeResult(1, "Request is cancelled");
            }
            else
            {
                result.Status = -1;
                result.Message = "Something is wrong. Please debug code.";
            }
            return result;
        }

        // Delete requests with status "rejected" or "cancelled" - User
        public async Task<IGiveandtakeResult> DeleteRequest(int requestId)
        {
            GiveandtakeResult result = new GiveandtakeResult();

            var request = await _unitOfWork.GetRepository<Request>().SingleOrDefaultAsync(
                predicate: c => c.RequestId == requestId);

            if (request == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Request not found"
                };
            }

            if (request.Status == "Accepted")
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Request has been accepted"
                };
            }

            _unitOfWork.GetRepository<Request>().DeleteAsync(request);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (isSuccessful)
            {
                result = new GiveandtakeResult(1, "Request is deleted");
            }
            else
            {
                result.Status = -1;
                result.Message = "Something is wrong. Please debug code.";
            }
            return result;
        }


        // Delete requests with status "rejected" or "cancelled" - Admin
        public async Task<IGiveandtakeResult> DeleteRejectedOrCancelledRequests()
        {
            GiveandtakeResult result = new GiveandtakeResult();

            // Lấy danh sách các request có status "rejected" hoặc "cancelled"
            var rejectedOrCancelledRequests = await _unitOfWork.GetRepository<Request>()
                .GetListAsync(predicate: r => r.Status == "Rejected" || r.Status == "Cancelled");

            if (rejectedOrCancelledRequests == null || !rejectedOrCancelledRequests.Any())
            {
                result.Status = -1;
                result.Message = "No requests found with status 'rejected' or 'cancelled'.";
                return result;
            }

            // Xóa các request này
            foreach (var request in rejectedOrCancelledRequests)
            {
                _unitOfWork.GetRepository<Request>().DeleteAsync(request);
            }

            // Commit thay đổi vào database
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (isSuccessful)
            {
                result.Status = 1;
                result.Message = "Deleted all requests with status 'rejected' or 'cancelled'.";
            }
            else
            {
                result.Status = -1;
                result.Message = "Failed to delete requests.";
            }

            return result;
        }
        #endregion

    }
}
