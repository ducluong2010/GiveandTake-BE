using System;
using System.Collections.Generic;

namespace GiveandTake_Repo.Models;

public partial class TransactionDetail
{
    public int TransactionDetailId { get; set; }

    public int? TransactionId { get; set; }

    public int? DonationId { get; set; }

    public int? Type { get; set; }

    public virtual Donation? Donation { get; set; }

    public virtual Transaction? Transaction { get; set; }
}
