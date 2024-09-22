using System;
using System.Collections.Generic;

namespace GiveandTake_Repo.Models;

public partial class Message
{
    public int MessageId { get; set; }

    public int? AccountId { get; set; }

    public string? Content { get; set; }

    public DateTime? SendAt { get; set; }

    public ulong? IsRead { get; set; }

    public virtual Account? Account { get; set; }
}
