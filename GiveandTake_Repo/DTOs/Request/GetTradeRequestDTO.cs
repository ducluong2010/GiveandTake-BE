using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveandTake_Repo.DTOs.Request
{
    public class GetTradeRequestDTO
    {
        public int TradeRequestId { get; set; }

        public int? AccountId { get; set; }

        public int? TradeDonationId { get; set; }

        public int? RequestDonationId { get; set; }

        public DateTime? RequestDate { get; set; }

        public string? Status { get; set; }
    }

    public class TradeRequestDTO
    {
        public int? AccountId { get; set; }

        public int? TradeDonationId { get; set; }

        public int? RequestDonationId { get; set; }

        public DateTime? RequestDate { get; set; }

        public string? Status { get; set; }
    }
}
