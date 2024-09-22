using System;
using System.Collections.Generic;

namespace GiveandTake_Repo.Models;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public int? TotalPoint { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<TransactionDetail> TransactionDetails { get; set; } = new List<TransactionDetail>();
}
