using System;
using System.Collections.Generic;

namespace GiveandTake_Repo.Models;

public partial class FeedbackMedium
{
    public int FeedbackMediaId { get; set; }

    public int? FeedbackId { get; set; }

    public string? MediaUrl { get; set; }

    public string? MediaType { get; set; }

    public virtual Feedback? Feedback { get; set; }
}
