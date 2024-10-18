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

        #region Manage Transaction

        [HttpPut(ApiEndPointConstant.Transaction.ChangeTransactionStatusToSuspendedEndPoint)]
        [SwaggerOperation(Summary = "Change transaction status to Suspended - Admin")]
        public async Task<IActionResult> ChangeTransactionStatusToSuspended(int id)
        {
            var response = await _transactionService.ChangeTransactionStatusToSuspended(id);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpPut(ApiEndPointConstant.Transaction.ChangeTransactionStatusToPendingEndPoint)]
        [SwaggerOperation(Summary = "Change transaction status to Pending - Admin")]
        public async Task<IActionResult> ChangeTransactionStatusToPending(int id)
        {
            var response = await _transactionService.ChangeTransactionStatusToPending(id);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpDelete(ApiEndPointConstant.Transaction.DeleteSuspendedTransactionEndPoint)]
        [SwaggerOperation(Summary = "Delete Suspended transaction - Admin")]
        public async Task<IActionResult> DeleteSuspendedTransaction(int id)
        {
            var response = await _transactionService.DeleteSuspendedTransaction(id);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpGet(ApiEndPointConstant.Transaction.TransactionByDonationForAdminEndPoint)]
        [SwaggerOperation(Summary = "Get list of transactions that user created - Admin")]
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
