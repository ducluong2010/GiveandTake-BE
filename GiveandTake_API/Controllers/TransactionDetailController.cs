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


        [HttpGet(ApiEndPointConstant.TransactionDetail.TransactionDetailByTransactionEndPoint)]
        [SwaggerOperation(Summary = "Get Transaction Detail by Transaction")]
        public async Task<IActionResult> GetTransactionDetailByTransactionId(int transactionId)
        {
            var response = await _transactionDetailService.GetTransactionDetailByTransactionId(transactionId);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpPost(ApiEndPointConstant.TransactionDetail.TransactionDetailsEndPoint)]
        [SwaggerOperation(Summary = "Generate QRCode by Transaction")]
        public async Task<IActionResult> GenerateQRCode(int transactiondetailid, int donationid)
        {
            if (transactiondetailid <= 0 || donationid <= 0)
            {
                return BadRequest("Invalid transactiondetailid or donationid");
            }

            var response = await _transactionDetailService.GenerateQRCode(transactiondetailid, donationid);
            return response.Status >= 0
                ? Ok(new { Message = response.Message, QRCodeData = response.Data })
                : BadRequest(response.Message);
        }

        [HttpGet(ApiEndPointConstant.TransactionDetail.GetQRCodeByTransactionId)]
        [SwaggerOperation(Summary = "Get QRCode by TransactionId")]
        public IActionResult GetQRCode(int transactionId, int donationId)
        {
            string directoryPath = Path.Combine("wwwroot", "images", "qrcodes");

            string fileName = $"qrcode_{transactionId}_{donationId}.png";
            string filePath = Path.Combine(directoryPath, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { message = "QR Code not found" });
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "image/png");
        }
    }
}
