using System;
using System.Collections.Generic;

namespace GiveandTake_Repo.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int? DonationId { get; set; }

    public int? AccountId { get; set; }

    public string? Note { get; set; }

    public string? Type { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? IsRead { get; set; }

    public int? StaffId { get; set; }
}
