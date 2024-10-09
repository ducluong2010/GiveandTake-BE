using GiveandTake_API.Constants;
using GiveandTake_Repo.DTOs.Transaction;
using Giveandtake_Services.Implements;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GiveandTake_API.Controllers
{
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly TransactionService _transactionService;
        public TransactionController( )
        {
            _transactionService = new TransactionService();
        }

        [HttpGet(ApiEndPointConstant.Transaction.TransactionsEndPoint)]
        [SwaggerOperation(Summary = "Get all Transactions")]
        public async Task<IActionResult> GetAllTransactions()
        {
            var response = await _transactionService.GetAllTransactions();
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpGet(ApiEndPointConstant.Transaction.TransactionEndPoint)]
        [SwaggerOperation(Summary = "Get Transaction by its id")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            var response = await _transactionService.GetTransactionById(id);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpGet(ApiEndPointConstant.Transaction.TransactionByAccountEndPoint)]
        [SwaggerOperation(Summary = "Get Transactions by Account")]
        public async Task<IActionResult> GetTransactionByAccount(int accountId)
        {
            var response = await _transactionService.GetTransactionByAccount(accountId);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpPost(ApiEndPointConstant.Transaction.TransactionsEndPoint)]
        [SwaggerOperation(Summary = "Create a new Transaction")]
        public async Task<IActionResult> CreateTransaction(TransactionDTO.CreateTransaction transactionInfo)
        {
            var response = await _transactionService.CreateTransaction(transactionInfo);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpPut(ApiEndPointConstant.Transaction.TransactionEndPoint)]
        [SwaggerOperation(Summary = "Update a Transaction")]
        public async Task<IActionResult> UpdateTransaction(int id, TransactionDTO.UpdateTransaction transactionInfo)
        {
            var response = await _transactionService.UpdateTransaction(id, transactionInfo);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpDelete(ApiEndPointConstant.Transaction.TransactionEndPoint)]
        [SwaggerOperation(Summary = "Delete a Transaction")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var response = await _transactionService.DeleteTransaction(id);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpPut(ApiEndPointConstant.Transaction.TransactionStatusEndPoint)]
        [SwaggerOperation(Summary = "Change Transaction Status")]
        public async Task<IActionResult> ChangeTransactionStatus(int id, string status)
        {
            await _transactionService.ChangeTransactionStatus(id, status);
            return Ok();
        }
    }
}
