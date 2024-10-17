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
    }
}
