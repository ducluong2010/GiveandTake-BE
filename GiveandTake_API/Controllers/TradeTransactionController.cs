using GiveandTake_API.Constants;
using Giveandtake_Services.Implements;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetAllTradeTransaction()
        {
            var response = await _tradeTransactionService.GetAllTradeTransaction();
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

 /*       [HttpGet(ApiEndPointConstant.TradeTransaction.TradeTransactionEndPoint)]
        public async Task<IActionResult> GetTradeTransactionById(int id)
        {
            var response = await _tradeTransactionService.GetTradeTransactionById(id);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }
 */
    }
}
