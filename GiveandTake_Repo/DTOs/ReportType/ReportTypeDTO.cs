using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveandTake_Repo.DTOs.ReportType
{
    public class ReportTypeDTO
    {
        public int ReportTypeId { get; set; }

        public string? ReportTypeName { get; set; }

        public string? Description { get; set; }

        public string? Status { get; set; }

    }

    public class ReportCreateTypeDTO
    {
        public string? ReportTypeName { get; set; }

        public string? Description { get; set; }

        public string? Status { get; set; }

    }

    public class ReportUpdateTypeDTO
    {
        public string? ReportTypeName { get; set; }

        public string? Description { get; set; }

        public string? Status { get; set; }

    }
}
