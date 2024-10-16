using GiveandTake_API.Constants;
using Giveandtake_Services.Implements;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GiveandTake_API.Controllers
{
    [ApiController]
    public class TransactionAdminController : ControllerBase
    {
        private readonly TransactionService _transactionService;

        public TransactionAdminController()
        {
            _transactionService = new TransactionService();
        }

        #region Admin Transaction

        // Change Transaction status to Suspended
        [HttpPut(ApiEndPointConstant.Transaction.ChangeTransactionStatusToSuspendedEndPoint)]
        [SwaggerOperation(Summary = "Change Transaction status to Suspended")]
        public async Task<IActionResult> ChangeTransactionStatusToSuspended(int id)
        {
            await _transactionService.ChangeTransactionStatusToSuspended(id);
            return Ok();
        }

        // Change Transaction status to Pending
        [HttpPut(ApiEndPointConstant.Transaction.ChangeTransactionStatusToPendingEndPoint)]
        [SwaggerOperation(Summary = "Change Transaction status to Pending")]
        public async Task<IActionResult> ChangeTransactionStatusToPending(int id)
        {
            await _transactionService.ChangeTransactionStatusToPending(id);
            return Ok();
        }

        // Delete Suspended Transaction
        [HttpDelete(ApiEndPointConstant.Transaction.DeleteSuspendedTransactionEndPoint)]
        [SwaggerOperation(Summary = "Delete Suspended Transaction")]
        public async Task<IActionResult> DeleteSuspendedTransaction(int id)
        {
            var response = await _transactionService.DeleteSuspendedTransaction(id);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        #endregion
    }
}
