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

        public DonationController(DonationService donationService)
        {
            _donationService = donationService;
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


        [HttpGet(ApiEndPointConstant.Donation.DonationsAppEndPoint)]
        [SwaggerOperation(Summary = "Get all Donations approved")]
        public async Task<IActionResult> GetAllApproved([FromQuery] int page = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _donationService.GetAllApproved(page, pageSize);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpGet(ApiEndPointConstant.Donation.DonationStaEndPoint)]
        [SwaggerOperation(Summary = "Get Donations by Staff")]
        public async Task<IActionResult> GetAllByStaff([FromQuery] int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _donationService.GetAllByStaff(id, page, pageSize);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpGet(ApiEndPointConstant.Donation.DonationClaimEndPoint)]
        [SwaggerOperation(Summary = "Get Claimed by Account")]
        public async Task<IActionResult> GetDonationsByAccountId([FromQuery] int id)
        {
            var response = await _donationService.GetDonationsByAccountId(id);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpGet(ApiEndPointConstant.Donation.DonationAccEndPoint)]
        [SwaggerOperation(Summary = "Get All Donations by Account")]
        public async Task<IActionResult> GetAllByAccountId([FromQuery] int id)
        {
            var response = await _donationService.GetAllByAccountId(id);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpGet(ApiEndPointConstant.Donation.DonationTypeEndPoint)]
        [SwaggerOperation(Summary = "Get Claimed by Account & Type")]
        public async Task<IActionResult> GetByAccountIdAndType([FromQuery] int id, int type)
        {
            var response = await _donationService.GetByAccountIdAndType(id, type);
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

        [HttpPost(ApiEndPointConstant.Donation.DonationsEndPoint)]
        [SwaggerOperation(Summary = "Create a new Donation")]
        public async Task<IActionResult> CreateDonation(CreateDonationDTO donation)
        {
            var response = await _donationService.CreateDonation(donation);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

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
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "2")]
        [HttpPut(ApiEndPointConstant.Donation.ToggleDonationStatusEndPoint)]
        [SwaggerOperation(Summary = "Change status between Pending and Approved")]
        public async Task<IActionResult> ToggleDonationStatus(int id)
        {
            var response = await _donationService.ToggleDonationStatus(id);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "2")] 
        [HttpPut(ApiEndPointConstant.Donation.ToggleCancelEndPoint)]
        [SwaggerOperation(Summary = "Change status between Cancel and Pending")]
        public async Task<IActionResult> ToggleCancel(int id)
        {
            var response = await _donationService.ToggleCancel(id);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "2")] 
        [HttpPut(ApiEndPointConstant.Donation.ToggleApprovedEndPoint)]
        [SwaggerOperation(Summary = "Change status between Approved and Cancel")]
        public async Task<IActionResult> ToggleApproved(int id)
        {
            var response = await _donationService.ToggleApproved(id);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "2")]
        [HttpPost(ApiEndPointConstant.Donation.CheckBannedAccountDonationsEndPoint)]
        [SwaggerOperation(Summary = "Check and update donations for all banned accounts")]
        public async Task<IActionResult> CheckAndUpdateAllBannedAccountsDonations()
        {
            var response = await _donationService.CheckAndUpdateAllBannedAccountsDonations();
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }
        [HttpPut(ApiEndPointConstant.Donation.ToggleTypeEndPoint)]
        [SwaggerOperation(Summary = "Change type 1 to 2")]
        public async Task<IActionResult> ToggleType(int id)
        {
            var response = await _donationService.ToggleType(id);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }
        [HttpPut(ApiEndPointConstant.Donation.ToggleType2EndPoint)]
        [SwaggerOperation(Summary = "Change type 1 to 3")]
        public async Task<IActionResult> ToggleType2(int id)
        {
            var response = await _donationService.ToggleType2(id);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }
        [HttpPut(ApiEndPointConstant.Donation.ToggleType3EndPoint)]
        [SwaggerOperation(Summary = "Change type 2 to 3")]
        public async Task<IActionResult> ToggleType3(int id)
        {
            var response = await _donationService.ToggleType3(id);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "2")]
        [HttpPost(ApiEndPointConstant.Donation.CheckHidingDonationsEndPoint)]
        [SwaggerOperation(Summary = "Check and update hiding donations for activated accounts")]
        public async Task<IActionResult> CheckAndUpdateDonationsForActivatedAccounts()
        {
            var response = await _donationService.CheckAndUpdateDonationsForActivatedAccounts();
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }
        [HttpGet(ApiEndPointConstant.Donation.SearchDonationsEndPoint)]
        [SwaggerOperation(Summary = "Search donations")]
        public async Task<IActionResult> SearchDonations([FromQuery] string searchTerm, [FromQuery] int page = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _donationService.SearchDonations(searchTerm, page, pageSize);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpPut(ApiEndPointConstant.Donation.DonationStatusEndPoint)]
        [SwaggerOperation(Summary = "Change Donation status")]
        public async Task<IActionResult> ChangeDonationStatus(int id, [FromBody] string newStatus)
        {
            var response = await _donationService.ChangeDonationStatus(id, newStatus);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

    }
}
