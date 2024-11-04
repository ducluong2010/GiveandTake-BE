using System;
using System.Collections.Generic;

namespace GiveandTake_Repo.Models;

public partial class TradeTransactionDetail
{
    public int TransactionDetailId { get; set; }

    public int? TransactionId { get; set; }

    public int? RequestDonationId { get; set; }

    public string? Qrcode { get; set; }

    public virtual Donation? RequestDonation { get; set; }

    public virtual TradeTransaction? Transaction { get; set; }
}
