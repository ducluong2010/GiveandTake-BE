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

        #region Sender Transaction

        [HttpGet(ApiEndPointConstant.Transaction.TransactionByDonationForSenderEndPoint)]
        [SwaggerOperation(Summary = "Get transactions that contain current logged in user items - Sender")]
        public async Task<IActionResult> GetTransactionsByDonationForSender()
        {
            int accountId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId").Value);

            var response = await _transactionService.GetTransactionsByDonationForSender(accountId);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpPut(ApiEndPointConstant.Transaction.ChangeTransactionStatusToAcceptedEndPoint)]
        [SwaggerOperation(Summary = "Accept the transaction - Sender")]
        public async Task<IActionResult> AcceptTransaction(int id)
        {
            int senderId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId").Value);

            var response = await _transactionService.ChangeTransactionStatusToAccepted(id, senderId);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response.Message);
        }

        [HttpPut(ApiEndPointConstant.Transaction.ChangeTransactionStatusToRejectedEndPoint)]
        [SwaggerOperation(Summary = "Reject the transaction - Sender")]
        public async Task<IActionResult> RejectTransaction(int id)
        {
            int senderId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId").Value);

            var response = await _transactionService.ChangeTransactionStatusToRejected(id, senderId);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response.Message);
        }

        [HttpPut(ApiEndPointConstant.Transaction.CompleteTransactionEndPoint)]
        [SwaggerOperation(Summary = "Complete the Transaction - Sender")]
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

        #region Receiver Transaction

        [HttpPost(ApiEndPointConstant.Transaction.CreateTransactionWithDetailEndPoint)]
        [SwaggerOperation(Summary = "Create transaction with details - Receiver")]
        public async Task<IActionResult> CreateTransactionWithDetail([FromBody] CreateTransactionWithDetail transactionInfo)
        {
            int accountId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId").Value);
            transactionInfo.Transaction.AccountId = accountId;

            var response = await _transactionService.CreateTransactionWithDetail(transactionInfo.Transaction, transactionInfo.TransactionDetail);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response.Message);
        }

        [HttpGet(ApiEndPointConstant.Transaction.TransactionByAccountForCurrentUserEndPoint)]
        [SwaggerOperation(Summary = "Get transactions created by current logged in user - Receiver")]
        public async Task<IActionResult> GetTransactionByCurrentUser()
        {
            int accountId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId").Value);

            var response = await _transactionService.GetTransactionByAccount(accountId);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        #endregion
    }
}
