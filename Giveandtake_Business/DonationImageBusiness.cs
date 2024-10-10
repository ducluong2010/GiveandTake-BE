using GiveandTake_Repo.DTOs.Donation;
using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Business
{
    public class DonationImageBusiness
    {
        public readonly UnitOfWork _unitOfWork;

        public DonationImageBusiness()
        {
            _unitOfWork = new UnitOfWork();
        }

        public async Task<IGiveandtakeResult> GetAllDonationImages()
        {
            var donationImageList = await _unitOfWork.GetRepository<DonationImage>()
                .GetListAsync(selector: x => new GetDonationImageDTO
                {
                    ImageId = x.ImageId,
                    DonationId = x.DonationId,
                    Url = x.Url,
                    IsThumbnail = x.IsThumbnail
                });
            return new GiveandtakeResult(donationImageList);
        }

        public async Task<IGiveandtakeResult> GetImageById(int id)
        {
            var donationImage = await _unitOfWork.GetRepository<DonationImage>()
                .SingleOrDefaultAsync(predicate: c => c.ImageId == id,
                                      selector: x => new GetDonationImageDTO
                                      {
                                          ImageId = x.ImageId,
                                          DonationId = x.DonationId,
                                          Url = x.Url,
                                          IsThumbnail = x.IsThumbnail
                                      });

            return new GiveandtakeResult(donationImage);
        }

        public async Task<IGiveandtakeResult> GetImagesByDonationId(int donationId)
        {
            var donationImages = await _unitOfWork.GetRepository<DonationImage>()
                .GetListAsync(predicate: c => c.DonationId == donationId,
                              selector: x => new GetDonationImageDTO
                              {
                                  ImageId = x.ImageId,
                                  DonationId = x.DonationId,
                                  Url = x.Url,
                                  IsThumbnail = x.IsThumbnail
                              });

            return new GiveandtakeResult(donationImages);
        }

        public async Task<IGiveandtakeResult> AddDonationImages(DonationImageDTO donationImageDTO)
        {
            GiveandtakeResult result = new GiveandtakeResult();

            // Check if another image with IsThumbnail = 1 already exists for the given DonationId
            var existingThumbnail = await _unitOfWork.GetRepository<DonationImage>()
                .FirstOrDefaultAsync(img => img.DonationId == donationImageDTO.DonationId && img.IsThumbnail == true);

            if (donationImageDTO.IsThumbnail == true && existingThumbnail != null)
            {
                result.Status = -1;
                result.Message = "Thumbnail already exists for this donation.";
                return result;
            }


            var donationImage = new DonationImage
            {
                DonationId = donationImageDTO.DonationId,
                Url = donationImageDTO.Url,
                IsThumbnail = donationImageDTO.IsThumbnail
            };

            await _unitOfWork.GetRepository<DonationImage>().InsertAsync(donationImage);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (isSuccessful)
            {
                result = new GiveandtakeResult(1, "Image added.");
            }
            else
            {
                result.Status = -1;
                result.Message = "Failed to add image.";
            }

            return result;
        }

        public async Task<IGiveandtakeResult> DeleteDonationImage(int id)
        {
            var donationImage = await _unitOfWork.GetRepository<DonationImage>()
                .SingleOrDefaultAsync(predicate: c => c.ImageId == id);

            if (donationImage == null)
            {
                return new GiveandtakeResult(-1, "Image not found.");
            }

            _unitOfWork.GetRepository<DonationImage>().DeleteAsync(donationImage);
            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult(1, "Image deleted successfully.");
        }

        public async Task<IGiveandtakeResult> ChangeThumbnail(int id)
        {
            var donationImage = await _unitOfWork.GetRepository<DonationImage>()
                .SingleOrDefaultAsync(predicate: c => c.ImageId == id);

            if (donationImage == null)
            {
                return new GiveandtakeResult(-1, "Image not found.");
            }

            var existingThumbnail = await _unitOfWork.GetRepository<DonationImage>()
                .FirstOrDefaultAsync(img => img.DonationId == donationImage.DonationId && img.IsThumbnail == true);

            if (existingThumbnail != null)
            {
                existingThumbnail.IsThumbnail = false;
                _unitOfWork.GetRepository<DonationImage>().UpdateAsync(existingThumbnail);
            }

            donationImage.IsThumbnail = true;
            _unitOfWork.GetRepository<DonationImage>().UpdateAsync(donationImage);
            await _unitOfWork.CommitAsync();

            return new GiveandtakeResult(1, "Thumbnail updated successfully.");
        }
    }
}
