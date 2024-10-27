using System;
using System.Collections.Generic;

namespace GiveandTake_Repo.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public string? Status { get; set; }

    public int Point { get; set; }

    public virtual ICollection<Donation> Donations { get; set; } = new List<Donation>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}
