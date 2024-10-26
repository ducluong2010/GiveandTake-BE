using System;
using System.Collections.Generic;

namespace GiveandTake_Repo.Models;

public partial class Favorite
{
    public int FavoriteId { get; set; }

    public int? AccountId { get; set; }

    public int? CategoryId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Category? Category { get; set; }
}
