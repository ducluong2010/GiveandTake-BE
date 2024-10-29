using System;
using System.Collections.Generic;

namespace GiveandTake_Repo.Models;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public int? AccountId { get; set; }

    public int? DonationId { get; set; }

    public int? Rating { get; set; }

    public string? Content { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? SenderId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Donation? Donation { get; set; }

    public virtual ICollection<FeedbackMedium> FeedbackMedia { get; set; } = new List<FeedbackMedium>();
}
