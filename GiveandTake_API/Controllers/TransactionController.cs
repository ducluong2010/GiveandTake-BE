﻿using GiveandTake_API.Constants;
using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Transaction;
using GiveandTake_Repo.Repository.Implements;
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

        #region Transaction

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

        [HttpGet(ApiEndPointConstant.Transaction.TransactionStatusEndPoint)]
        [SwaggerOperation(Summary = "Get Transaction Status")]
        public async Task<IActionResult> GetTransactionStatus()
        {
           
            if (!int.TryParse(RouteData.Values["id"]?.ToString(), out int id))
            {
                return BadRequest("Invalid transaction ID");
            }

            var response = await _transactionService.GetTransactionStatus(id);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpGet(ApiEndPointConstant.Transaction.CompletedTransactionsByDonationForSenderEndPoint)]
        [SwaggerOperation(Summary = "Get Completed Transaction By Account")]
        public async Task<IActionResult> GetCompletedTransactionsByDonationForSender(int accountId)
        {
            var response = await _transactionService.GetCompletedTransactionsByAccountId(accountId);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpPut(ApiEndPointConstant.Transaction.FeedbackChangeEndPoint)]
        [SwaggerOperation(Summary = "Toggle feedback status of a Transaction")]
        public async Task<IActionResult> ToggleIsFeedbackStatus([FromRoute] int transactionId)
        {
            var response = await _transactionService.ToggleIsFeedbackStatus(transactionId);
            if (response.Status >= 0)
                return Ok(response.Message);
            else
                return BadRequest(response);
        }
        #endregion
    }
}
