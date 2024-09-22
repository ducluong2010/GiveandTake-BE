using System;
using System.Collections.Generic;

namespace GiveandTake_Repo.Models;

public partial class UserRole
{
    public int RoleId { get; set; }

    public string? RoleName { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
