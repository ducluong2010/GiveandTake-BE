using System;
using System.Collections.Generic;

namespace GiveandTake_Repo.Models;

public partial class DonationImage
{
    public int ImageId { get; set; }

    public int? DonationId { get; set; }

    public string? Url { get; set; }

    public bool? IsThumbnail { get; set; }

    public virtual Donation? Donation { get; set; }
}
