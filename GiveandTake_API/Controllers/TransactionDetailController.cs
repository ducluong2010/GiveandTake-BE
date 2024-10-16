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
        public async Task<IActionResult> GetTransactionDetailById(int transactionDetailId)
        {
            var response = await _transactionDetailService.GetTransactionDetailById(transactionDetailId);
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

        #region Unused methods
        //[HttpPost(ApiEndPointConstant.TransactionDetail.TransactionDetailsEndPoint)]
        //[SwaggerOperation(Summary = "Create a new Transaction Detail")]
        //public async Task<IActionResult> CreateTransactionDetail(TransactionDetailDTO transactionDetail)
        //{
        //    var response = await _transactionDetailService.CreateTransactionDetail(transactionDetail);
        //    if (response.Status >= 0)
        //        return Ok(response);
        //    else
        //        return BadRequest(response);
        //}

        //[HttpPut(ApiEndPointConstant.TransactionDetail.TransactionDetailEndPoint)]
        //[SwaggerOperation(Summary = "Update a Transaction Detail")]
        //public async Task<IActionResult> UpdateTransactionDetail(int transactionDetailId, TransactionDetailDTO transactionDetail)
        //{
        //    var response = await _transactionDetailService.UpdateTransactionDetail(transactionDetailId, transactionDetail);
        //    if (response.Status >= 0)
        //        return Ok(response);
        //    else
        //        return BadRequest(response);
        //}

        //[HttpDelete(ApiEndPointConstant.TransactionDetail.TransactionDetailEndPoint)]
        //[SwaggerOperation(Summary = "Delete a Transaction Detail")]
        //public async Task<IActionResult> DeleteTransactionDetail(int transactionDetailId)
        //{
        //    var response = await _transactionDetailService.DeleteTransactionDetail(transactionDetailId);
        //    if (response.Status >= 0)
        //        return Ok(response);
        //    else
        //        return BadRequest(response);
        //}
        #endregion
    }
}
