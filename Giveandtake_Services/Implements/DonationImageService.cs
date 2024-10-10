using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Donation;
using Giveandtake_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Implements
{
    public class DonationImageService : IDonationImageService
    {
        private readonly DonationImageBusiness _donationImageBusiness;
        public DonationImageService()
        {
            _donationImageBusiness = new DonationImageBusiness();
        }

        public Task<IGiveandtakeResult> AddDonationImages(DonationImageDTO donationImageDTO)
            => _donationImageBusiness.AddDonationImages(donationImageDTO);

        public Task<IGiveandtakeResult> ChangeThumbnail(int id)
            => _donationImageBusiness.ChangeThumbnail(id);

        public Task<IGiveandtakeResult> DeleteDonationImage(int id)
            => _donationImageBusiness.DeleteDonationImage(id);

        public Task<IGiveandtakeResult> GetAllDonationImages()
            => _donationImageBusiness.GetAllDonationImages();

        public Task<IGiveandtakeResult> GetImageById(int id)
            => _donationImageBusiness.GetImageById(id);

        public Task<IGiveandtakeResult> GetImagesByDonationId(int donationId)
            => _donationImageBusiness.GetImagesByDonationId(donationId);
    }
}
