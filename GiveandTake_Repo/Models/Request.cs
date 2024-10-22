using System;
using System.Collections.Generic;

namespace GiveandTake_Repo.Models;

public partial class Request
{
    public int RequestId { get; set; }

    public int? AccountId { get; set; }

    public int? DonationId { get; set; }

    public DateTime? RequestDate { get; set; }

    public string? Status { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Donation? Donation { get; set; }
}
