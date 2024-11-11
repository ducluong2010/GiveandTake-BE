using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveandTake_Repo.DTOs.Report
{
    public class ReportDTO
    {
        public int? SenderId { get; set; }

        public string? SenderName { get; set; }

        public int ReportId { get; set; }

        public int? AccountId { get; set; }

        public string? AccountName { get; set; }

        public int? DonationId { get; set; }

        public string? DonationName { get; set; }

        public string? Description { get; set; }

        public int? ReportTypeId { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? ReportTypeName { get; set; } 

        public List<string> ReportMediaUrls { get; set; } = new List<string>(); 
    }

    public class ReportCreateDTO
    {
        public int? SenderId { get; set; }

        public int? AccountId { get; set; }

        public string? Description { get; set; }

        public int? ReportTypeId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public List<string> ReportMediaUrls { get; set; } = new List<string>();
    }

    public class ReportUpdateDTO
    {
        public string? Description { get; set; } 
        public int? ReportTypeId { get; set; } 
        public List<string> ReportMediaUrls { get; set; } = new List<string>(); 
    }
}
