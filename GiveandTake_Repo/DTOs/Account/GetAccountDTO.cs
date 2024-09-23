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

        public ulong? IsActive { get; set; }
    }

    public class AccountDTO
    {
        public int? RoleId { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public int? Point { get; set; }

        public string? AvatarUrl { get; set; }

        public ulong? IsActive { get; set; }
    }
}
