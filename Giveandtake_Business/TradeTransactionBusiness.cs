using GiveandTake_Repo.DTOs.TradeTransaction;
using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GiveandTake_Repo.DTOs.TradeTransaction.TradeTransactionDTO;
using static GiveandTake_Repo.DTOs.Transaction.TransactionDTO;
using TradeTransaction = GiveandTake_Repo.Models.TradeTransaction;

namespace Giveandtake_Business
{
    public class TradeTransactionBusiness
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly TradeTransactionDetailBusiness _tradeTransactionDetailBusiness;

        public TradeTransactionBusiness()
        {
            _unitOfWork = new UnitOfWork();
            _tradeTransactionDetailBusiness = new TradeTransactionDetailBusiness();
        }

        #region Basic Trade Transaction

        // Get all trade transactions
        public async Task<IGiveandtakeResult> GetAllTradeTransaction()
        {
            var tradeList = await _unitOfWork.GetRepository<TradeTransaction>()
                .GetListAsync(selector: o => new GetTradeTransaction()
                {
                    TradeTransactionId = o.TradeTransactionId,
                    AccountId = o.AccountId,
                    TradeDonationId = o.TradeDonationId,
                    CreatedDate = o.CreatedDate,
                    UpdatedDate = o.UpdatedDate,
                    Status = o.Status
                });
            return new GiveandtakeResult(tradeList);
        }

        // Get trade transaction by id
        public async Task<IGiveandtakeResult> GetTradeTransactionById(int id)
        {
            var tradeTransaction = await _unitOfWork.GetRepository<TradeTransaction>()
                .SingleOrDefaultAsync(
                    predicate: o => o.TradeTransactionId == id,
                    selector: o => new
                    {
                        TradeTransaction = new GetTradeTransaction()
                        {
                            TradeTransactionId = o.TradeTransactionId,
                            AccountId = o.AccountId,
                            TradeDonationId = o.TradeDonationId,
                            CreatedDate = o.CreatedDate,
                            UpdatedDate = o.UpdatedDate,
                            Status = o.Status
                        },
                        TradeTransactionDetail = o.TradeTransactionDetail == null ? null : new GetTradeTransactionDetailDTO()
                        {
                            TradeTransactionDetailId = o.TradeTransactionDetail.TradeTransactionDetailId,
                            TradeTransactionId = o.TradeTransactionDetail.TradeTransactionId,
                            RequestDonationId = o.TradeTransactionDetail.RequestDonationId,
                            Qrcode = o.TradeTransactionDetail.Qrcode,
                        }
                    },
                    include: source => source
                        .Include(o => o.Account)
                        .Include(o => o.TradeDonation)
                        .Include(o => o.TradeTransactionDetail)
                );

            if (tradeTransaction == null)
            {
                return new GiveandtakeResult(-1, "Trade transaction not found");
            }

            return new GiveandtakeResult(new
            {
                TradeTransaction = tradeTransaction.TradeTransaction,
                TradeTransactionDetail = tradeTransaction.TradeTransactionDetail
            });
        }

        #endregion
    }
}
