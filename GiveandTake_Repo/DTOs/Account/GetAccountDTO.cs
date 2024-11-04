using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveandTake_Repo.DTOs.Account
{
    public class GetAccountDTO
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

        public DateTime? PremiumUnti { get; set; }

        public double? Rating { get; set; }

        public int? ChatId { get; set; }

        public int? MessageId { get; set; }

        public int? ActiveTime { get; set; }

        public string? Otp {  get; set; }

    }

    public class AccountDTO
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

        public DateTime? PremiumUnti { get; set; }

        public double? Rating { get; set; }

        public int? ChatId { get; set; }

        public int? MessageId { get; set; }

        public int? ActiveTime { get; set; }

        public string? Otp { get; set; }

    }
}
