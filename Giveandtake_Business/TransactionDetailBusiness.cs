using GiveandTake_Repo.DTOs.Transaction;
using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebaseAdmin;
using Google.Cloud.Storage.V1;
using Google.Apis.Auth.OAuth2;

namespace Giveandtake_Business
{
    public class TransactionDetailBusiness
    {
        private readonly UnitOfWork _unitOfWork;

        public TransactionDetailBusiness()
        {
            _unitOfWork = new UnitOfWork();
        }

        #region TransactionDetail

        // Get all transaction details
        public async Task<IGiveandtakeResult> GetAllTransactionDetail()
        {
            var transactionDetailList = await _unitOfWork.GetRepository<TransactionDetail>()
                .GetListAsync(selector: x => new GetTransactionDetailDTO
                {
                    TransactionDetailId = x.TransactionDetailId,
                    TransactionId = x.TransactionId,
                    DonationId = x.DonationId,
                    Qrcode = x.Qrcode
                });
            return new GiveandtakeResult(transactionDetailList);
        }

        // Get transaction detail by its id
        public async Task<IGiveandtakeResult> GetTransactionDetailById(int transactionDetailId)
        {
            var transactionDetail = await _unitOfWork.GetRepository<TransactionDetail>()
                .SingleOrDefaultAsync(predicate: c => c.TransactionDetailId == transactionDetailId,
                                      selector: x => new GetTransactionDetailDTO
                                      {
                                          TransactionDetailId = x.TransactionDetailId,
                                          TransactionId = x.TransactionId,
                                          DonationId = x.DonationId,
                                          Qrcode = x.Qrcode
                                      });
            return new GiveandtakeResult(transactionDetail);
        }

        // Get transaction detail by transaction id
        public async Task<IGiveandtakeResult> GetTransactionDetailByTransactionId(int transactionId)
        {
            var transactionDetail = await _unitOfWork.GetRepository<TransactionDetail>()
                .GetListAsync(predicate: c => c.TransactionId == transactionId,
                              selector: x => new GetTransactionDetailDTO
                              {
                                  TransactionDetailId = x.TransactionDetailId,
                                  TransactionId = x.TransactionId,
                                  DonationId = x.DonationId,
                                  Qrcode = x.Qrcode
                              });
            return new GiveandtakeResult(transactionDetail);
        }

        // Create a new transaction detail
        public async Task<IGiveandtakeResult> CreateTransactionDetail(TransactionDetailDTO transactionDetail)
        {
            var newTransactionDetail = new TransactionDetail
            {
                TransactionId = transactionDetail.TransactionId,
                DonationId = transactionDetail.DonationId
            };

            await _unitOfWork.GetRepository<TransactionDetail>().InsertAsync(newTransactionDetail);

            GiveandtakeResult result = new GiveandtakeResult();

            bool status = await _unitOfWork.CommitAsync() > 0;
            if (status)
            {
                result.Status = 1;
                result.Message = "Transaction Detail created successfully";
            }
            else
            {
                result.Status = -1;
                result.Message = "Transaction Detail creation failed";
            }
            return result;
        }

        // Update a transaction detail
        public async Task<IGiveandtakeResult> UpdateTransactionDetail(int transactionDetailId, TransactionDetailDTO transactionDetail)
        {
            var transactionDetailToUpdate = await _unitOfWork.GetRepository<TransactionDetail>().SingleOrDefaultAsync(predicate: c => c.TransactionDetailId == transactionDetailId);

            if (transactionDetailToUpdate == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Transaction Detail not found"
                };
            }

            transactionDetailToUpdate.TransactionId = transactionDetail.TransactionId;
            transactionDetailToUpdate.DonationId = transactionDetail.DonationId;

            _unitOfWork.GetRepository<TransactionDetail>().UpdateAsync(transactionDetailToUpdate);

            GiveandtakeResult result = new GiveandtakeResult();

            bool status = await _unitOfWork.CommitAsync() > 0;
            if (status)
            {
                result.Status = 1;
                result.Message = "Transaction Detail updated successfully";
            }
            else
            {
                result.Status = -1;
                result.Message = "Transaction Detail update failed";
            }
            return result;
        }

        // Delete a transaction detail
        public async Task<IGiveandtakeResult> DeleteTransactionDetail(int transactionDetailId)
        {
            TransactionDetail transactionDetail = await _unitOfWork.GetRepository<TransactionDetail>()
                .SingleOrDefaultAsync(predicate: c => c.TransactionDetailId == transactionDetailId);
            if (transactionDetail == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Transaction Detail not found"
                };
            }
            else
            {
                _unitOfWork.GetRepository<TransactionDetail>().DeleteAsync(transactionDetail);
                await _unitOfWork.CommitAsync();
            }
            return new GiveandtakeResult(1, "Delete Successfully");
        }

        #endregion TransactionDetail

        // Generate QRCode for transaction
        public async Task<IGiveandtakeResult> GenerateQRCode(int transactionId, int transactionDetailId, int donationId)
        {
            // Get Information from DonationId
            var donation = await _unitOfWork.GetRepository<Donation>().SingleOrDefaultAsync(predicate: d => d.DonationId == donationId);
            if (donation == null)
            {
                return new GiveandtakeResult { Status = -1, Message = "Donation not found" };
            }

            // Get Information form AccountId
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: a => a.AccountId == donation.AccountId);
            if (account == null)
            {
                return new GiveandtakeResult { Status = -1, Message = "Account not found" };
            }

            // Get TransactionDetail from DonationId
            var transactionDetail = await _unitOfWork.GetRepository<TransactionDetail>()
                .SingleOrDefaultAsync(predicate: td => td.DonationId == donationId);
            if (transactionDetail == null)
            {
                return new GiveandtakeResult { Status = -1, Message = "Transaction Detail not found" };
            }

            // Create Info in QRCode
            string shortInfo = $"Transaction ID: {transactionId}\n" +
                               $"Donation ID: {donationId}\n" +
                               $"Donation Name: {donation.Name}\n" +
                               $"Account Name: {account.FullName}";


            string adminSdkPath = Path.Combine(Directory.GetCurrentDirectory(), "adminsdk.json");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", adminSdkPath);

            try
            {
                // Generate QRCode
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(shortInfo, QRCodeGenerator.ECCLevel.Q);
                BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
                byte[] qrCodeBytes = qrCode.GetGraphic(20);

                // Create file name based on transactionId and donationId
                string fileName = $"qrcode_{transactionId}_{donationId}.png";

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
                string downloadUrl = urlSigner.Sign(bucketName, objectName, TimeSpan.FromHours(1), HttpMethod.Get);

                // Save link to database
                transactionDetail.Qrcode = downloadUrl;
                _unitOfWork.GetRepository<TransactionDetail>().UpdateAsync(transactionDetail);
                bool status = await _unitOfWork.CommitAsync() > 0;

                if (status)
                {
                    return new GiveandtakeResult
                    {
                        Status = 1,
                        Message = "QR Code generated and uploaded to Firebase successfully"
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

        public async Task<IGiveandtakeResult> GetQrcodeByTransactionDetailId(int transactionDetailId)
        {
            var result = new GiveandtakeResult();

            // Logic lấy thông tin chi tiết của transaction detail từ database
            var transactionDetail = await _unitOfWork.GetRepository<TransactionDetail>()
            .SingleOrDefaultAsync(predicate: td => td.TransactionDetailId == transactionDetailId);

            if (transactionDetail != null)
            {
                // Check if the QR code exists
                if (!string.IsNullOrEmpty(transactionDetail.Qrcode))
                {
                    result.Status = 1;
                    result.Data = transactionDetail.Qrcode; // Return the QR code
                    result.Message = "QR code found.";
                }
                else
                {
                    result.Status = 0;
                    result.Message = "QR code not found.";
                }
            }
            else
            {
                result.Status = 0;
                result.Message = "Transaction detail not found.";
            }

            return result;
        }
    }
}
