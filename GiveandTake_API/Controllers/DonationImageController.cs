using GiveandTake_API.Constants;
using GiveandTake_Repo.DTOs.Donation;
using Giveandtake_Services.Implements;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GiveandTake_API.Controllers
{
    [ApiController]
    public class DonationImageController : ControllerBase
    {
        private readonly DonationImageService _donationImageService;

        public DonationImageController()
        {
            _donationImageService = new DonationImageService();
        }

        [HttpGet(ApiEndPointConstant.DonationImage.DonationImagesEndPoint)]
        [SwaggerOperation(Summary = "Get all Donation Images")]
        public async Task<IActionResult> GetAllDonationImages()
        {
            var response = await _donationImageService.GetAllDonationImages();
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }

        [HttpGet(ApiEndPointConstant.DonationImage.DonationAllImageEndPoint)]
        [SwaggerOperation(Summary = "Get Donation Image by donation id")]
        public async Task<IActionResult> GetImagesByDonationId(int donationId)
        {
            var response = await _donationImageService.GetImagesByDonationId(donationId);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }

        [HttpGet(ApiEndPointConstant.DonationImage.DonationImageEndPoint)]
        [SwaggerOperation(Summary = "Get Donation Image by its id")]
        public async Task<IActionResult> GetImageById(int id)
        {
            var response = await _donationImageService.GetImageById(id);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }

        [HttpPost(ApiEndPointConstant.DonationImage.DonationImagesEndPoint)]
        [SwaggerOperation(Summary = "Add a new Donation Image")]
        public async Task<IActionResult> AddDonationImages(DonationImageDTO donationImageDTO)
        {
            var response = await _donationImageService.AddDonationImages(donationImageDTO);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpDelete(ApiEndPointConstant.DonationImage.DonationImageEndPoint)]
        [SwaggerOperation(Summary = "Delete a Donation Image by its id")]
        public async Task<IActionResult> DeleteDonationImage(int id)
        {
            var response = await _donationImageService.DeleteDonationImage(id);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpPut(ApiEndPointConstant.DonationImage.ChangeThumbnailEndPoint)]
        [SwaggerOperation(Summary = "Change Thumbnail of a Donation Image")]
        public async Task<IActionResult> ChangeThumbnail(int id)
        {
            var response = await _donationImageService.ChangeThumbnail(id);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }
    }
}
