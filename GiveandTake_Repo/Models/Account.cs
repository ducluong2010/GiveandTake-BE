using System;
using System.Collections.Generic;

namespace GiveandTake_Repo.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public int? RoleId { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public int? TransactionId { get; set; }

    public int? Point { get; set; }

    public string? AvatarUrl { get; set; }

    public ulong? IsActive { get; set; }

    public virtual ICollection<Donation> Donations { get; set; } = new List<Donation>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Rewarded> Rewardeds { get; set; } = new List<Rewarded>();

    public virtual ICollection<Reward> Rewards { get; set; } = new List<Reward>();

    public virtual UserRole? Role { get; set; }

    public virtual Transaction? Transaction { get; set; }
}
