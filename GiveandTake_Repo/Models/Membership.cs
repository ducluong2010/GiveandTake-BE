using System;
using System.Collections.Generic;

namespace GiveandTake_Repo.Models;

public partial class Membership
{
    public int MembershipId { get; set; }

    public int? AccountId { get; set; }

    public DateTime? PurchaseDate { get; set; }

    public DateTime? PremiumUntil { get; set; }

    public string? Status { get; set; }
    public int TransactionId { get; set; }
    public virtual Account? Account { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
}
