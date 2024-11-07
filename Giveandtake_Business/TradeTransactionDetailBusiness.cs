using FirebaseAdmin;
using GiveandTake_Repo.DTOs.TradeTransaction;
using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Business
{
    public class TradeTransactionDetailBusiness
    {
        private readonly UnitOfWork _unitOfWork;
        public TradeTransactionDetailBusiness()
        {
            _unitOfWork = new UnitOfWork();
        }

        #region TradeTransactionDetail

        // Get All TradeTransactionDetail
        public async Task<IGiveandtakeResult> GetAllTradeTransactionDetail()
        {
            var tradeDetailList = await _unitOfWork.GetRepository<TradeTransactionDetail>()
                .GetListAsync(selector: x => new GetTradeTransactionDetailDTO
                {
                    TradeTransactionDetailId = x.TradeTransactionDetailId,
                    TradeTransactionId = x.TradeTransactionId,
                    RequestDonationId = x.RequestDonationId,
                    Qrcode = x.Qrcode,
                });
            return new GiveandtakeResult(tradeDetailList);
        }

        // Get TradeTransactionDetail By Id
        public async Task<IGiveandtakeResult> GetTradeTransactionDetailById(int id)
        {
            var tradeDetail = await _unitOfWork.GetRepository<TradeTransactionDetail>()
                .SingleOrDefaultAsync(predicate: c => c.TradeTransactionDetailId == id,
                                      selector: x => new GetTradeTransactionDetailDTO
                                      {
                                          TradeTransactionDetailId = x.TradeTransactionDetailId,
                                          TradeTransactionId = x.TradeTransactionId,
                                          RequestDonationId = x.RequestDonationId,
                                          Qrcode = x.Qrcode,
                                      });
            return new GiveandtakeResult(tradeDetail);
        }

        // Get TradeTransactionDetail By TradeTransactionId
        public async Task<IGiveandtakeResult> GetTradeTransactionDetailByTradeTransactionId(int tradeId)
        {
            var tradeDetail = await _unitOfWork.GetRepository<TradeTransactionDetail>()
                .GetListAsync(predicate: c => c.TradeTransactionId == tradeId,
                              selector: x => new GetTradeTransactionDetailDTO
                              {
                                  TradeTransactionDetailId = x.TradeTransactionDetailId,
                                  TradeTransactionId = x.TradeTransactionId,
                                  RequestDonationId = x.RequestDonationId,
                                  Qrcode = x.Qrcode,
                              });
            return new GiveandtakeResult(tradeDetail);
        }

        // Create TradeTransactionDetail
        public async Task<IGiveandtakeResult> CreateTradeTransactionDetail(TradeTransactionDetailDTO tradeDetail)
        {
            var newTradeDetail = new TradeTransactionDetail
            {
                TradeTransactionId = tradeDetail.TradeTransactionId,
                RequestDonationId = tradeDetail.RequestDonationId,
            };

            _unitOfWork.GetRepository<TradeTransactionDetail>().InsertAsync(newTradeDetail);
            GiveandtakeResult result = new GiveandtakeResult();

            bool status = await _unitOfWork.CommitAsync() > 0;
            if (status)
            {
                result.Status = 1;
                result.Message = "Trade Detail created successfully";
            }
            else
            {
                result.Status = -1;
                result.Message = "Trade Detail created failed";
            }
            return result;
        }

        // Delete TradeTransactionDetail
        public async Task<IGiveandtakeResult> DeleteTradeTransactionDetail(int id)
        {
            var tradeDetail = await _unitOfWork.GetRepository<TradeTransactionDetail>()
                .SingleOrDefaultAsync(predicate: c => c.TradeTransactionDetailId == id);
            if (tradeDetail == null)
            {
                return new GiveandtakeResult(-1, "Trade Detail not found");
            }

            _unitOfWork.GetRepository<TradeTransactionDetail>().DeleteAsync(tradeDetail);
            GiveandtakeResult result = new GiveandtakeResult();

            bool status = await _unitOfWork.CommitAsync() > 0;
            if (status)
            {
                result.Status = 1;
                result.Message = "Trade Detail deleted successfully";
            }
            else
            {
                result.Status = -1;
                result.Message = "Trade Detail deleted failed";
            }
            return result;
        }

        #endregion

        #region QRCode
        // Generate QRCode for trade transaction detail
        public async Task<IGiveandtakeResult> GenerateQRCode(int tradeTransactionId, int tradeTransactionDetailId, int requestDonationId)
        {
            // Get Information from RequestDonationId
            var requestDonation = await _unitOfWork.GetRepository<TradeTransactionDetail>().SingleOrDefaultAsync(predicate: rd => rd.RequestDonationId == requestDonationId);
            if (requestDonation == null)
            {
                return new GiveandtakeResult { Status = -1, Message = "Request Donation not found" };
            }

            // Get TradeTransactionDetail from RequestDonationId
            var tradeTransactionDetail = await _unitOfWork.GetRepository<TradeTransactionDetail>()
                .SingleOrDefaultAsync(predicate: ttd => ttd.RequestDonationId == requestDonationId);
            if (tradeTransactionDetail == null)
            {
                return new GiveandtakeResult { Status = -1, Message = "Trade Transaction Detail not found" };
            }

            // Create Info in QRCode
            string shortInfo = $"trade_transaction_Id: {tradeTransactionId}\n" +
                               $"request_donation_Id: {requestDonationId}";

            string adminSdkPath = Path.Combine(Directory.GetCurrentDirectory(), "adminsdk.json");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", adminSdkPath);

            try
            {
                // Generate QRCode
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(shortInfo, QRCodeGenerator.ECCLevel.Q);
                BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
                byte[] qrCodeBytes = qrCode.GetGraphic(20);

                // Create file name based on tradeTransactionId and requestDonationId
                string fileName = $"qrcode_{tradeTransactionId}_{requestDonationId}.png";

                // Initialize Firebase Admin SDK (if not already initialized)
                if (FirebaseApp.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile(adminSdkPath)
                    });
                }

                // Create a single credential instance
                var credential = GoogleCredential.FromFile(adminSdkPath)
                    .CreateScoped(Google.Apis.Storage.v1.StorageService.Scope.CloudPlatform);

                // Create Storage client
                StorageClient storageClient = StorageClient.Create(credential);

                // Your Google Cloud Storage bucket name
                string bucketName = "qrcode-5543f.appspot.com";

                // Upload to Google Cloud Storage
                using (var stream = new MemoryStream(qrCodeBytes))
                {
                    await storageClient.UploadObjectAsync(bucketName, $"qrcodes/{fileName}", "image/png", stream);
                }

                // Generate download URL (this URL will be public and valid for a limited time)
                var urlSigner = UrlSigner.FromCredential(credential);
                string objectName = $"qrcodes/{fileName}";
                string downloadUrl = urlSigner.Sign(bucketName, objectName, TimeSpan.FromHours(24), HttpMethod.Get);

                // Save link to database
                tradeTransactionDetail.Qrcode = downloadUrl;
                _unitOfWork.GetRepository<TradeTransactionDetail>().UpdateAsync(tradeTransactionDetail);
                bool status = await _unitOfWork.CommitAsync() > 0;

                if (status)
                {
                    return new GiveandtakeResult
                    {
                        Status = 1,
                        Message = "QR Code generated and uploaded to Firebase successfully",
                        Data = downloadUrl // Return QRCode URL
                    };
                }
                else
                {
                    return new GiveandtakeResult
                    {
                        Status = -1,
                        Message = "Failed to save QR Code URL to database"
                    };
                }
            }
            catch (Exception ex)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = $"QR Code generation or upload failed: {ex.Message}"
                };

            }
        }

        public async Task<IGiveandtakeResult> GetQrcodeByTradeTransactionId(int tradeTransactionId)
        {
            var result = new GiveandtakeResult();

            var tradeTransactionDetail = await _unitOfWork.GetRepository<TradeTransactionDetail>()
                .SingleOrDefaultAsync(predicate: ttd => ttd.TradeTransactionId == tradeTransactionId);

            if (tradeTransactionDetail != null)
            {
                if (!string.IsNullOrEmpty(tradeTransactionDetail.Qrcode))
                {
                    result.Status = 1;
                    result.Message = "QR Code found";
                    result.Data = tradeTransactionDetail.Qrcode;
                }
                else
                {
                    result.Status = -1;
                    result.Message = "QR Code not found";
                }
            }
            else
            {
                result.Status = 0;
                result.Message = "Trade Transaction not found";
            }
            return result;
        }
        #endregion
    }
}
