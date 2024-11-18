using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveandTake_Repo.DTOs.Member
{
    public class MembershipDTO
    {
        public int MembershipId { get; set; }
        public int? AccountId { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? PremiumUntil { get; set; }
        public string? Status { get; set; }

        public string? Amount { get; set; }
        public string? FullName { get; set; }
    }

    public class CreateMembershipDTO
    {
        public int MembershipId { get; set; }

        public int? AccountId { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public DateTime? PremiumUntil { get; set; }

        public string? Status { get; set; }

        public string? Amount { get; set; }
    }

    public class UpdateMembershipDTO
    {
        public int? AccountId { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public DateTime? PremiumUntil { get; set; }

        public string? Status { get; set; }

        public string? Amount { get; set; }
    }
}
