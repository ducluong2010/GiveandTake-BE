using System;
using System.Collections.Generic;

namespace GiveandTake_Repo.Models;

public partial class ReportType
{
    public int ReportTypeId { get; set; }

    public string? ReportTypeName { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
}
