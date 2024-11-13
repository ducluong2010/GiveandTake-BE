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
                .GetListAsync(
                    predicate: c => c.DonationId == donationId,
                    selector: x => new
                    {
                        Request = new GetRequestDTO
                        {
                            RequestId = x.RequestId,
                            AccountId = x.AccountId,
                            DonationId = x.DonationId,
                            RequestDate = x.RequestDate,
                            Status = x.Status
                        },
                        IsPremium = x.Account.IsPremium
                    });

            var sortedRequestList = requestList
                .OrderByDescending(x => x.IsPremium)
                .ThenBy(x => x.Request.RequestDate)
                .Select(x => x.Request)
                .ToList();

            return new GiveandtakeResult(sortedRequestList);
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

            // Kiểm tra tài khoản
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: c => c.AccountId == requestDTO.AccountId);
            if (account == null)
            {
                return new GiveandtakeResult(-1, "Không tìm thấy id của tài khoản.");
            }

            // Kiểm tra donation
            var donation = await _unitOfWork.GetRepository<Donation>().SingleOrDefaultAsync(
                predicate: d => d.DonationId == requestDTO.DonationId && d.Status == "Approved");
            if (donation == null)
            {
                return new GiveandtakeResult(-1, "Không tìm thấy món đồ hoặc nó chưa được staff duyệt qua.");
            }

            // Kiểm tra nếu người dùng yêu cầu cho donation của chính họ
            if (donation.AccountId == requestDTO.AccountId)
            {
                return new GiveandtakeResult(-1, "Bạn không thể request chính món đồ của mình.");
            }

            // Kiểm tra xem người dùng đã tạo request cho donation này chưa
            var existingRequest = await _unitOfWork.GetRepository<Request>().SingleOrDefaultAsync(
                predicate: r => r.DonationId == requestDTO.DonationId && r.AccountId == requestDTO.AccountId);
            if (existingRequest != null)
            {
                return new GiveandtakeResult(-1, "Bạn đã request món đồ này rồi.");
            }

            // Nếu người dùng không phải Premium, kiểm tra xem có transaction nào completed trong vòng 7 ngày gần nhất không
            if ((bool)!account.IsPremium)
            {
                var recentTransaction = await _unitOfWork.GetRepository<Transaction>().SingleOrDefaultAsync(
                    predicate: t => t.AccountId == requestDTO.AccountId &&
                                    t.Status == "Completed" &&
                                    t.UpdatedDate >= DateTime.Now.AddDays(-7));

                if (recentTransaction != null)
                {
                    return new GiveandtakeResult(-1, "Bạn phải chờ 7 ngày kể từ giao dịch gần nhất, mới có thể request tiếp được.");
                }
            }

            // Tạo request mới
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
                result = new GiveandtakeResult(1, "Tạo request thành công. Xin hãy chờ phản hồi của chủ món đồ.");
            }
            else
            {
                result.Status = -1;
                result.Message = "Tạo request không thành công, đã có lỗi xảy ra.";
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
                    Message = "Không tìm thấy request để cancel hoặc bạn không có quyền cancel request này."
                };
            }

            if (request.Status == "Cancelled")
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Request đã được cancel trước đó rồi."
                };
            }

            if (request.Status == "Accepted")
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Request đã được chủ món đồ chấp nhận rồi."
                };
            }

            request.Status = "Cancelled";
            _unitOfWork.GetRepository<Request>().UpdateAsync(request);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (isSuccessful)
            {
                result = new GiveandtakeResult(1, "Request đã được cancel thành công.");
            }
            else
            {
                result.Status = -1;
                result.Message = "Đã có lỗi xảy ra.";
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
                    Message = "Không tìm thấy request để xóa"
                };
            }

            if (request.Status == "Accepted")
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Request đã được chủ món đồ chấp nhận, không thể xóa"
                };
            }

            _unitOfWork.GetRepository<Request>().DeleteAsync(request);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (isSuccessful)
            {
                result = new GiveandtakeResult(1, "Request đã được xóa thành công");
            }
            else
            {
                result.Status = -1;
                result.Message = "Đã có lỗi xảy ra.";
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
