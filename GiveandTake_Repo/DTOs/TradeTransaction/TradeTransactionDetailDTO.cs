using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveandTake_Repo.DTOs.TradeTransaction
{
    public class TradeTransactionDetailDTO
    {
        public int? TradeTransactionId { get; set; }

        public int? RequestDonationId { get; set; }

        public string? Qrcode { get; set; }
    }

    public class GetTradeTransactionDetailDTO
    {
        public int TradeTransactionDetailId { get; set; }

        public int? TradeTransactionId { get; set; }

        public int? RequestDonationId { get; set; }

        public string? Qrcode { get; set; }

    }
}
