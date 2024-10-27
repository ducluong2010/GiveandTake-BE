using System;
using System.Collections.Generic;

namespace GiveandTake_Repo.Models;

public partial class ReportMedium
{
    public int ReportMediaId { get; set; }

    public int? ReportId { get; set; }

    public string? ReportUrl { get; set; }

    public virtual Report? Report { get; set; }
}
