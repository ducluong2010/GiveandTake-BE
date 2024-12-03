using GiveandTake_API.Constants;
using GiveandTake_Repo.DTOs.Request;
using Giveandtake_Services.Implements;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GiveandTake_API.Controllers
{
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly RequestService _requestService;
        public RequestController()
        {
            _requestService = new RequestService();
        }

        [HttpGet(ApiEndPointConstant.Request.RequestsEndPoint)]
        [SwaggerOperation(Summary = "Get all requests")]
        public async Task<IActionResult> GetAllRequests()
        {
            var response = await _requestService.GetAllRequests();
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }

        [HttpGet(ApiEndPointConstant.Request.RequestEndPoint)]
        [SwaggerOperation(Summary = "Get request by its id")]
        public async Task<IActionResult> GetRequestInfo(int id)
        {
            var response = await _requestService.GetRequestById(id);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }

        [HttpGet(ApiEndPointConstant.Request.RequestByAccountEndPoint)]
        [SwaggerOperation(Summary = "Get request by account id")]
        public async Task<IActionResult> GetRequestByAccount(int accountId)
        {
            var response = await _requestService.GetRequestByAccountId(accountId);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }

        [HttpGet(ApiEndPointConstant.Request.RequestByDonationEndPoint)]
        [SwaggerOperation(Summary = "Get request by donation id")]
        public async Task<IActionResult> GetRequestByDonation(int donationId)
        {
            var response = await _requestService.GetRequestByDonationId(donationId);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }

        //[HttpPut(ApiEndPointConstant.Request.CancelRequestEndPoint)]
        //[SwaggerOperation(Summary = "Cancel request")]
        //public async Task<IActionResult> CancelRequest(int id)
        //{
        //    int receiverId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId").Value);

        //    var response = await _requestService.CancelRequest(id, receiverId);
        //    if (response.Status >= 0)
        //        return Ok(response);
        //    else
        //        return BadRequest(response);
        //}

        [HttpPost(ApiEndPointConstant.Request.CreateRequestEndPoint)]
        [SwaggerOperation(Summary = "Create request")]
        public async Task<IActionResult> CreateRequest(RequestDTO requestInfo)
        {
            int accountId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId").Value);

            requestInfo.AccountId = accountId;

            var response = await _requestService.CreateRequest(requestInfo);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpDelete(ApiEndPointConstant.Request.RequestEndPoint)]
        [SwaggerOperation(Summary = "Delete request")]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            var response = await _requestService.DeleteRequest(id);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpPut(ApiEndPointConstant.Request.CancelRequestEndPoint)]
        [SwaggerOperation(Summary = "Cancel all request by donation id")]
        public async Task<IActionResult> CancelRequestsByDonationId(int id)
        {
            var response = await _requestService.CancelRequestsByDonationId(id);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }
    }
}
