using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveandTake_Repo.DTOs.Feedback
{
    public class FeedbackDTO
    {
        public int FeedbackId { get; set; }

        public int? AccountId { get; set; }

        public string? AccountName { get; set; }  

        public int? DonationId { get; set; }

        public string? DonationName { get; set; } 

        public int? Rating { get; set; }

        public string? Content { get; set; }

        public DateTime? CreatedDate { get; set; }

        public List<string>? FeedbackMediaUrls { get; set; }
    }

    public class CreateFeedbackDTO
    {
        public int? AccountId { get; set; }

        public int? DonationId { get; set; }

        public int? Rating { get; set; }

        public string? Content { get; set; }

        public DateTime? CreateTime { get; set; }

        public List<string>? FeedbackMediaUrls { get; set; } 
    }
}
