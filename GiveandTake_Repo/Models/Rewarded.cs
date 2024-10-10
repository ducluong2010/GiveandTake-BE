using System;
using System.Collections.Generic;

namespace GiveandTake_Repo.Models;

public partial class Rewarded
{
    public int RewardedId { get; set; }

    public int? RewardId { get; set; }

    public int? AccountId { get; set; }

    public string? Status { get; set; }

    public DateTime? ClaimedAt { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Reward? Reward { get; set; }
}
