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

    public int? Point { get; set; }

    public string? AvatarUrl { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsPremium { get; set; }

    public DateTime? PremiumUntil { get; set; }

    public double? Rating { get; set; }

    public int? ChatId { get; set; }

    public int? MessageId { get; set; }

    public int? ActiveTime { get; set; }

    public string? Otp { get; set; }

    public virtual ICollection<Donation> Donations { get; set; } = new List<Donation>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual ICollection<Rewarded> Rewardeds { get; set; } = new List<Rewarded>();

    public virtual ICollection<Reward> Rewards { get; set; } = new List<Reward>();

    public virtual UserRole? Role { get; set; }

    public virtual ICollection<TradeRequest> TradeRequests { get; set; } = new List<TradeRequest>();
}
