using System;
using System.Collections.Generic;

namespace GiveandTake_Repo.Models;

public partial class Donation
{
    public int DonationId { get; set; }

    public int? AccountId { get; set; }

    public int? CategoryId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public int? Point { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? ApprovedBy { get; set; }

    public int? TotalRating { get; set; }

    public string? Status { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<DonationImage> DonationImages { get; set; } = new List<DonationImage>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<TransactionDetail> TransactionDetails { get; set; } = new List<TransactionDetail>();
}
