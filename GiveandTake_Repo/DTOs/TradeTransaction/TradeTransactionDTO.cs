using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveandTake_Repo.DTOs.TradeTransaction
{
    public class TradeTransactionDTO
    {
        public class CreateTradeTransaction
        {
            public int? AccountId { get; set; }

            public int? TradeDonationId { get; set; }

            public DateTime? CreatedDate { get; set; }

            public DateTime? UpdatedDate { get; set; }

            public string? Status { get; set; }
        }

        public class GetTradeTransaction
        {
            public int TradeTransactionId { get; set; }

            public int? AccountId { get; set; }

            public int? TradeDonationId { get; set; }

            public DateTime? CreatedDate { get; set; }

            public DateTime? UpdatedDate { get; set; }

            public string? Status { get; set; }

        }
    }
}
