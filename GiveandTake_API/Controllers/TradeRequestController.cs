using GiveandTake_API.Constants;
using GiveandTake_Repo.DTOs.Request;
using Giveandtake_Services.Implements;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GiveandTake_API.Controllers
{
    [ApiController]
    public class TradeRequestController : ControllerBase
    {
        private readonly TradeRequestService _tradeRequestService;
        public TradeRequestController()
        {
            _tradeRequestService = new TradeRequestService();
        }

        [HttpGet(ApiEndPointConstant.TradeRequest.TradeRequestsEndPoint)]
        [SwaggerOperation(Summary = "Get all trade requests")]
        public async Task<IActionResult> GetAllTradeRequests()
        {
            var response = await _tradeRequestService.GetAllTradeRequests();
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }

        [HttpGet(ApiEndPointConstant.TradeRequest.TradeRequestEndPoint)]
        [SwaggerOperation(Summary = "Get trade request by its id")]
        public async Task<IActionResult> GetTradeRequestInfo(int id)
        {
            var response = await _tradeRequestService.GetTradeRequestById(id);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }

        [HttpGet(ApiEndPointConstant.TradeRequest.TradeRequestByAccountEndPoint)]
        [SwaggerOperation(Summary = "Get trade request by account id")]
        public async Task<IActionResult> GetTradeRequestByAccount(int accountId)
        {
            var response = await _tradeRequestService.GetTradeRequestByAccountId(accountId);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }

        [HttpGet(ApiEndPointConstant.TradeRequest.TradeRequestByDonationEndPoint)]
        [SwaggerOperation(Summary = "Get trade request by request donation id")]
        public async Task<IActionResult> GetTradeRequestByTradeDonation(int requestDonationId)
        {
            var response = await _tradeRequestService.GetTradeRequestByTradeDonationId(requestDonationId);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }

        [HttpPost(ApiEndPointConstant.TradeRequest.CreateTradeRequestEndPoint)]
        [SwaggerOperation(Summary = "Create trade request")]
        public async Task<IActionResult> CreateTradeRequest([FromBody] TradeRequestDTO tradeRequestDTO)
        {
            int accountId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId").Value);
            tradeRequestDTO.AccountId = accountId;

            var response = await _tradeRequestService.CreateTradeRequest(tradeRequestDTO);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }

        [HttpPut(ApiEndPointConstant.TradeRequest.CancelTradeRequestEndPoint)]
        [SwaggerOperation(Summary = "Cancel trade request")]
        public async Task<IActionResult> CancelTradeRequest(int id)
        {
            int accountId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId").Value);

            var response = await _tradeRequestService.CancelTradeRequest(id, accountId);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }

        [HttpDelete(ApiEndPointConstant.TradeRequest.DeleteTradeRequestEndPoint)]
        [SwaggerOperation(Summary = "Delete trade request")]
        public async Task<IActionResult> DeleteTradeRequest(int id)
        {
            var response = await _tradeRequestService.DeleteTradeRequest(id);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }
    }
}
