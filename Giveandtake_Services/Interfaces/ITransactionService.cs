﻿using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GiveandTake_Repo.DTOs.Transaction.TransactionDTO;

namespace Giveandtake_Services.Interfaces
{
    public interface ITransactionService
    {
        Task<IGiveandtakeResult> GetAllTransactions();
        Task<IGiveandtakeResult> GetTransactionById(int id);
        Task<IGiveandtakeResult> GetTransactionByAccount (int accountId);
        Task<IGiveandtakeResult> GetTransactionsByDonationForSender(int senderAccountId);
        Task<IGiveandtakeResult> GetCompletedTransactionsByAccountId(int senderAccountId);
        Task<IGiveandtakeResult> GetTransactionStatus(int transactionId);

        // Specifid methods for user
        Task<IGiveandtakeResult> CreateTransactionWithDetail(CreateTransaction createTransaction, TransactionDetailDTO transactionDetailDto, int senderAccountId);
        Task<IGiveandtakeResult> CompleteTransaction(int transactionId, int senderAccountId);
        Task<IGiveandtakeResult> ToggleIsFeedbackStatus(int transactionId);
        Task<IGiveandtakeResult> CancelTransaction(int transactionId, int senderAccountId);
    }
}
