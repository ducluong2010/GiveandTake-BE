using System;
using System.Collections.Generic;

namespace GiveandTake_Repo.Models;

public partial class TradeRequest
{
    public int TradeRequestId { get; set; }

    public int? AccountId { get; set; }

    public int? TradeDonationId { get; set; }

    public int? RequestDonationId { get; set; }

    public DateTime? RequestDate { get; set; }

    public string? Status { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Donation? RequestDonation { get; set; }

    public virtual Donation? TradeDonation { get; set; }
}
