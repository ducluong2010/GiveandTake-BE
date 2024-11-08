using System;
using System.Collections.Generic;

namespace GiveandTake_Repo.Models;

public partial class TradeTransactionDetail
{
    public int TradeTransactionDetailId { get; set; }

    public int? TradeTransactionId { get; set; }

    public int? RequestDonationId { get; set; }

    public string? Qrcode { get; set; }

    public virtual Donation? RequestDonation { get; set; }

    public virtual TradeTransaction? TradeTransaction { get; set; }
}
