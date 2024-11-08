using GiveandTake_API.Constants;
using Giveandtake_Services.Implements;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GiveandTake_API.Controllers
{
    [ApiController]
    public class TradeTransactionDetailController : ControllerBase
    {
        private readonly TradeTransactionDetailService _tradeTransactionDetailService;
        public TradeTransactionDetailController()
        {
            _tradeTransactionDetailService = new TradeTransactionDetailService();
        }

        [HttpGet(ApiEndPointConstant.TradeTransactionDetail.TradeTransactionDetailsEndPoint)]
        [SwaggerOperation(Summary = "Get all Trade Transaction Details")]
        public async Task<IActionResult> GetAllTradeTransactionDetail()
        {
            var response = await _tradeTransactionDetailService.GetAllTradeTransactionDetail();
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpGet(ApiEndPointConstant.TradeTransactionDetail.TradeTransactionDetailEndPoint)]
        [SwaggerOperation(Summary = "Get Trade Transaction Detail by its id")]
        public async Task<IActionResult> GetTradeTransactionDetailById(int id)
        {
            var response = await _tradeTransactionDetailService.GetTradeTransactionDetailById(id);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpGet(ApiEndPointConstant.TradeTransactionDetail.TradeTransactionDetailByTradeTransactionEndPoint)]
        [SwaggerOperation(Summary = "Get Trade Transaction Detail by Trade Transaction id")]
        public async Task<IActionResult> GetTradeTransactionDetailByTradeTransactionId(int tradeId)
        {
            var response = await _tradeTransactionDetailService.GetTradeTransactionDetailByTradeTransactionId(tradeId);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpPost(ApiEndPointConstant.TradeTransactionDetail.TradeTransactionDetailsEndPoint)]
        [SwaggerOperation(Summary = "Generate QRCode by Trade Transaction")]
        public async Task<IActionResult> GenerateQRCode(int tradeTransactionId, int tradeTransactionDetailId, int requestDonationId)
        {
            // Validate the input
            if (tradeTransactionDetailId <= 0 || requestDonationId <= 0)
            {
                return BadRequest("Invalid tradeTransactionId or requestDonationId");
            }

            // Call the business logic to generate QR code
            var response = await _tradeTransactionDetailService.GenerateQRCode(tradeTransactionId, tradeTransactionDetailId, requestDonationId);

            // Return the appropriate response based on the result
            if (response.Status >= 0)
            {
                return Ok(new { Message = response.Message, QRCodeUrl = response.Data });
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        [HttpGet(ApiEndPointConstant.TradeTransactionDetail.GetTradeQRCodeByTradeTransactionId)]
        [SwaggerOperation(Summary = "Get QRCode by Trade Transaction Id")]
        public async Task<IActionResult> GetTradeQRCode(int tradeTransactionId)
        {
            var response = await _tradeTransactionDetailService.GetQrcodeByTradeTransactionId(tradeTransactionId);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }
    }
}
