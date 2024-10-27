using System;
using System.Collections.Generic;

namespace GiveandTake_Repo.Models;

public partial class Report
{
    public int ReportId { get; set; }

    public int? AccountId { get; set; }

    public string? Description { get; set; }

    public int? ReportTypeId { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Account? Account { get; set; }

    public virtual ICollection<ReportMedium> ReportMedia { get; set; } = new List<ReportMedium>();

    public virtual ReportType? ReportType { get; set; }
}
