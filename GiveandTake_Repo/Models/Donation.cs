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

    public int? Type { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<DonationImage> DonationImages { get; set; } = new List<DonationImage>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual ICollection<TradeRequest> TradeRequestRequestDonations { get; set; } = new List<TradeRequest>();

    public virtual ICollection<TradeRequest> TradeRequestTradeDonations { get; set; } = new List<TradeRequest>();

    public virtual ICollection<TradeTransactionDetail> TradeTransactionDetails { get; set; } = new List<TradeTransactionDetail>();

    public virtual ICollection<TradeTransaction> TradeTransactions { get; set; } = new List<TradeTransaction>();

    public virtual ICollection<TransactionDetail> TransactionDetails { get; set; } = new List<TransactionDetail>();
}
