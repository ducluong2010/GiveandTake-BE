using System;
using System.Collections.Generic;

namespace GiveandTake_Repo.Models;

public partial class Reward
{
    public int RewardId { get; set; }

    public int? AccountId { get; set; }

    public string? RewardName { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public int? Point { get; set; }

    public int? Quantity { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? Status { get; set; }

    public bool? IsPremium { get; set; }

    public virtual Account? Account { get; set; }

    public virtual ICollection<Rewarded> Rewardeds { get; set; } = new List<Rewarded>();
}
