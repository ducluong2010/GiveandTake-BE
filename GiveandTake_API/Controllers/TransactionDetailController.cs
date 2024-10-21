using GiveandTake_API.Constants;
using GiveandTake_Repo.DTOs.Transaction;
using Giveandtake_Services.Implements;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GiveandTake_API.Controllers
{
    [ApiController]
    public class TransactionDetailController : ControllerBase
    {
        private readonly TransactionDetailService _transactionDetailService;
        public TransactionDetailController()
        {
            _transactionDetailService = new TransactionDetailService();
        }

        [HttpGet(ApiEndPointConstant.TransactionDetail.TransactionDetailsEndPoint)]
        [SwaggerOperation(Summary = "Get all Transaction Details")]
        public async Task<IActionResult> GetAllTransactionDetail()
        {
            var response = await _transactionDetailService.GetAllTransactionDetail();
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpGet(ApiEndPointConstant.TransactionDetail.TransactionDetailEndPoint)]
        [SwaggerOperation(Summary = "Get Transaction Detail by its id")]
        public async Task<IActionResult> GetTransactionDetailById(int id)
        {
            var response = await _transactionDetailService.GetTransactionDetailById(id);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }


        [HttpPost(ApiEndPointConstant.TransactionDetail.TransactionDetailsEndPoint)]
        [SwaggerOperation(Summary = "Generate QRCode by Transaction")]
        public async Task<IActionResult> GenerateQRCode(int transactionId,int transactionDetailId, int donationId)
        {
            // Validate the input
            if (transactionDetailId <= 0 || donationId <= 0)
            {
                return BadRequest("Invalid transactionId or donationId");
            }

            // Call the business logic to generate QR code
            var response = await _transactionDetailService.GenerateQRCode(transactionId, transactionDetailId, donationId);

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

        [HttpGet(ApiEndPointConstant.TransactionDetail.GetQRCodeByTransactionId)]
        [SwaggerOperation(Summary = "Get QRCode by TransactionId")]
        public async Task<IActionResult> GetQRCode(int transactionId)
        {
            if (transactionId <= 0)
            {
                return BadRequest("Invalid transactionId");
            }

            // Retrieve the transaction detail by transactionId
            var transactionDetail = await _transactionDetailService.GetTransactionDetailByTransactionId(transactionId);

            if (transactionDetail == null || string.IsNullOrEmpty(transactionDetail.Qrcode))
            {
                return NotFound(new { message = "QR Code not found" });
            }

            // Return the QRCode URL if it exists
            return Ok(new { QRCodeUrl = transactionDetail.Qrcode});
        }
    }
}
