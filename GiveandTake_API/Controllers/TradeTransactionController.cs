using GiveandTake_API.Constants;
using Giveandtake_Services.Implements;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GiveandTake_API.Controllers
{
    [ApiController]
    public class TradeTransactionController : ControllerBase
    {
        private readonly TradeTransactionService _tradeTransactionService;
        public TradeTransactionController()
        {
            _tradeTransactionService = new TradeTransactionService();
        }

        [HttpGet(ApiEndPointConstant.TradeTransaction.TradeTransactionsEndPoint)]
        [SwaggerOperation(Summary = "Get all trade transactions")]
        public async Task<IActionResult> GetAllTradeTransaction()
        {
            var response = await _tradeTransactionService.GetAllTradeTransaction();
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpGet(ApiEndPointConstant.TradeTransaction.TradeTransactionEndPoint)]
        [SwaggerOperation(Summary = "Get trade transaction by id")]
        public async Task<IActionResult> GetTradeTransactionById(int id)
        {
            var response = await _tradeTransactionService.GetTradeTransactionById(id);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpGet(ApiEndPointConstant.TradeTransaction.GetTradeTransactionStatusEndPoint)]
        [SwaggerOperation(Summary = "Get trade transaction status")]
        public async Task<IActionResult> GetTradeTransactionStatus(int id)
        {
            var response = await _tradeTransactionService.GetTradeTransactionStatus(id);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpGet(ApiEndPointConstant.TradeTransaction.GetTradeTransactionByAccountIdEndPoint)]
        [SwaggerOperation(Summary = "Get trade transaction by account id")]
        public async Task<IActionResult> GetTradeTransactionByAccountId(int accountId)
        {
            var response = await _tradeTransactionService.GetTradeTransactionByAccountId(accountId);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpPost(ApiEndPointConstant.TradeTransaction.AcceptTradeRequestEndPoint)]
        [SwaggerOperation(Summary = "Accept trade request, create trade transaction")]
        public async Task<IActionResult> AcceptTradeRequest(int tradeRequestId)
        {
            int loggedInAccountId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId").Value);

            var response = await _tradeTransactionService.AcceptTradeRequest(tradeRequestId, loggedInAccountId);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response.Message);
        }

        [HttpPut(ApiEndPointConstant.TradeTransaction.RejectTradeRequestEndPoint)]
        [SwaggerOperation(Summary = "Reject trade request")]
        public async Task<IActionResult> RejectTradeRequest(int tradeRequestId)
        {
            int loggedInAccountId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId").Value);

            var response = await _tradeTransactionService.RejectTradeRequest(tradeRequestId, loggedInAccountId);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response.Message);
        }

        [HttpPut(ApiEndPointConstant.TradeTransaction.CompleteTradeTransactionEndPoint)]
        [SwaggerOperation(Summary = "Complete trade transaction")]
        public async Task<IActionResult> CompleteTradeTransaction(int tradeTransactionId)
        {
            int loggedInAccountId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId").Value);

            var response = await _tradeTransactionService.CompleteTradeTransaction(tradeTransactionId, loggedInAccountId);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response.Message);
        }
    }
}
