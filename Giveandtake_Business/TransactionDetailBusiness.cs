using GiveandTake_Repo.DTOs.Transaction;
using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    DonationId = x.DonationId
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
                                          DonationId = x.DonationId
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
                                  DonationId = x.DonationId
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
        public async Task<IGiveandtakeResult> GenerateQRCode(int transactionId, int donationId)
        {
            // Get Information from DonationId
            var donation = await _unitOfWork.GetRepository<Donation>().SingleOrDefaultAsync(predicate: d => d.DonationId == donationId);

            if (donation == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Donation not found"
                };
            }

            // Get Information form AccountId
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: a => a.AccountId == donation.AccountId);
            if (account == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Account not found"
                };
            }

            // Get TransactionDetail from DonationId
            var transactionDetail = await _unitOfWork.GetRepository<TransactionDetail>()
                .SingleOrDefaultAsync(predicate: td => td.DonationId == donationId);

            if (transactionDetail == null)
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "Transaction Detail not found"
                };
            }

            // Create Info in QRCode
            string shortInfo = $"Transaction ID: {transactionId}\n" +
                               $"Donation ID: {donationId}\n" +
                               $"Donation Name: {donation.Name}\n" +
                               $"Account Name: {account.FullName}";

            // Generate QRCode
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(shortInfo, QRCodeGenerator.ECCLevel.Q);
            BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
            byte[] qrCodeBytes = qrCode.GetGraphic(20);

            // File Path to save img
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "qrcodes");

            // Create Path does not exist
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Create file name base on transactionId và donationId
            string fileName = $"qrcode_{transactionId}_{donationId}.png";
            string filePath = Path.Combine(directoryPath, fileName);  

            // Save img file to server
            File.WriteAllBytes(filePath, qrCodeBytes);

            // Save link img to database ("images/qrcodes/")
            transactionDetail.Qrcode = $"/images/qrcodes/{fileName}";
            _unitOfWork.GetRepository<TransactionDetail>().UpdateAsync(transactionDetail);

            bool status = await _unitOfWork.CommitAsync() > 0;
            if (status)
            {
                return new GiveandtakeResult
                {
                    Status = 1,
                    Message = "QR Code generated and saved as an image file successfully"
                };
            }
            else
            {
                return new GiveandtakeResult
                {
                    Status = -1,
                    Message = "QR Code generation failed"
                };
            }
        }
    }
}
