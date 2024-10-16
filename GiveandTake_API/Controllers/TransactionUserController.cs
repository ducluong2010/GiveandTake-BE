using GiveandTake_API.Constants;
using GiveandTake_Repo.DTOs.Transaction;
using Giveandtake_Services.Implements;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GiveandTake_API.Controllers
{
    [ApiController]
    public class TransactionUserController : ControllerBase
    {
        private readonly TransactionService _transactionService;
        public TransactionUserController()
        {
            _transactionService = new TransactionService();
        }

        #region User Transaction

        // Create Transaction with Detail
        [HttpPost(ApiEndPointConstant.Transaction.CreateTransactionWithDetailEndPoint)]
        [SwaggerOperation(Summary = "Create Transaction with Transaction Details")]
        public async Task<IActionResult> CreateTransactionWithDetail([FromBody] CreateTransactionWithDetail transactionInfo)
        {
            var response = await _transactionService.CreateTransactionWithDetail(transactionInfo.Transaction, transactionInfo.TransactionDetail);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response.Message);
        }


        // Get Transactions by Donation for Sender (user)
        [HttpGet(ApiEndPointConstant.Transaction.TransactionByDonationForSenderEndPoint)]
        [SwaggerOperation(Summary = "Get Transactions by Donation for Sender (User)")]
        public async Task<IActionResult> GetTransactionsByDonationForSender()
        {
            // Lấy account ID từ JWT token
            int accountId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId").Value);

            var response = await _transactionService.GetTransactionsByDonationForSender(accountId);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        // Change Transaction status to Accepted
        [HttpPut(ApiEndPointConstant.Transaction.ChangeTransactionStatusToAcceptedEndPoint)]
        [SwaggerOperation(Summary = "Change Transaction status to Accepted")]
        public async Task<IActionResult> AcceptTransaction(int id)
        {
            int senderId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId").Value);

            var response = await _transactionService.ChangeTransactionStatusToAccepted(id, senderId);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response.Message);
        }

        // Change Transaction status to Rejected
        [HttpPut(ApiEndPointConstant.Transaction.ChangeTransactionStatusToRejectedEndPoint)]
        [SwaggerOperation(Summary = "Change Transaction status to Rejected")]
        public async Task<IActionResult> RejectTransaction(int id)
        {
            int senderId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId").Value);

            var response = await _transactionService.ChangeTransactionStatusToRejected(id, senderId);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response.Message);
        }

        // Complete Transaction
        [HttpPut(ApiEndPointConstant.Transaction.CompleteTransactionEndPoint)]
        [SwaggerOperation(Summary = "Complete Transaction")]
        public async Task<IActionResult> CompleteTransaction(int id)
        {
            int senderId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId").Value);

            var response = await _transactionService.CompleteTransaction(id, senderId);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response.Message);
        }
        #endregion
    }
}
