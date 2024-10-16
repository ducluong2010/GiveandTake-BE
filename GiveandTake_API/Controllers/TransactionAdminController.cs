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
            var response = await _transactionService.ChangeTransactionStatusToSuspended(id);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

        // Change Transaction status to Pending
        [HttpPut(ApiEndPointConstant.Transaction.ChangeTransactionStatusToPendingEndPoint)]
        [SwaggerOperation(Summary = "Change Transaction status to Pending")]
        public async Task<IActionResult> ChangeTransactionStatusToPending(int id)
        {
            var response = await _transactionService.ChangeTransactionStatusToPending(id);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

        // Delete Suspended Transaction
        [HttpDelete(ApiEndPointConstant.Transaction.DeleteSuspendedTransactionEndPoint)]
        [SwaggerOperation(Summary = "Delete Suspended Transaction")]
        public async Task<IActionResult> DeleteSuspendedTransaction(int id)
        {
            var response = await _transactionService.DeleteSuspendedTransaction(id);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

        // Get Transactions by Donation for Sender (admin)
        [HttpGet(ApiEndPointConstant.Transaction.TransactionByDonationForAdminEndPoint)]
        [SwaggerOperation(Summary = "Get Transactions by Donation for Sender By Admin")]
        public async Task<IActionResult> GetTransactionsByDonationForSender(int senderAccountId)
        {
            var response = await _transactionService.GetTransactionsByDonationForSender(senderAccountId);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }
        #endregion
    }
}
