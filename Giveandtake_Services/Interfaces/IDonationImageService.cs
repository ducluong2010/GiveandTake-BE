using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Donation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Interfaces
{
    public interface IDonationImageService
    {
        Task<IGiveandtakeResult> GetAllDonationImages();
        Task<IGiveandtakeResult> GetImagesByDonationId(int donationId);
        Task<IGiveandtakeResult> GetImageById(int id);
        Task<IGiveandtakeResult> AddDonationImages(DonationImageDTO donationImageDTO);
        Task<IGiveandtakeResult> DeleteDonationImage(int id);
        Task<IGiveandtakeResult> ChangeThumbnail(int id);
    }
}
