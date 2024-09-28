using GiveandTake_API.Constants;
using GiveandTake_Repo.DTOs.Donation;
using Giveandtake_Services.Implements;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GiveandTake_API.Controllers
{
    [ApiController]
    public class DonationController : Controller
    {
        private readonly DonationService _donationService;

        public DonationController()
        {
            _donationService = new DonationService();
        }

        [HttpGet(ApiEndPointConstant.Donation.DonationsEndPoint)]
        [SwaggerOperation(Summary = "Get all Donations with pagination")]
        public async Task<IActionResult> GetAllDonations([FromQuery] int page = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _donationService.GetAllDonations(page, pageSize);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpGet(ApiEndPointConstant.Donation.DonationEndPoint)]
        [SwaggerOperation(Summary = "Get Donation by its id")]
        public async Task<IActionResult> GetDonationById(int id)
        {
            var response = await _donationService.GetDonationById(id);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "1")]
        [HttpPost(ApiEndPointConstant.Donation.DonationsEndPoint)]
        [SwaggerOperation(Summary = "Create a new Donation")]
        public async Task<IActionResult> CreateDonation(CreateUpdateDonationDTO donation)
        {
            var response = await _donationService.CreateDonation(donation);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "1")]
        [HttpPut(ApiEndPointConstant.Donation.DonationEndPoint)]
        [SwaggerOperation(Summary = "Update Donation")]
        public async Task<IActionResult> UpdateDonation(int id, CreateUpdateDonationDTO donation)
        {
            var response = await _donationService.UpdateDonation(id, donation);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "1")]
        [HttpDelete(ApiEndPointConstant.Donation.DonationEndPoint)]
        [SwaggerOperation(Summary = "Delete Donation")]
        public async Task<IActionResult> DeleteDonation(int id)
        {
            var response = await _donationService.DeleteDonation(id);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }
    }
}
