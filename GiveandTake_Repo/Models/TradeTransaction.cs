using System;
using System.Collections.Generic;

namespace GiveandTake_Repo.Models;

public partial class TradeTransaction
{
    public int TradeTransactionId { get; set; }

    public int? AccountId { get; set; }

    public int? TradeDonationId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? Status { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Donation? TradeDonation { get; set; }

    public virtual ICollection<TradeTransactionDetail> TradeTransactionDetails { get; set; } = new List<TradeTransactionDetail>();
}
