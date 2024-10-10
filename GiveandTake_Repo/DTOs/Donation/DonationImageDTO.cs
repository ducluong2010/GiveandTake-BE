using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveandTake_Repo.DTOs.Donation
{
    public class DonationImageDTO
    {
        public int? DonationId { get; set; }

        public string? Url { get; set; }

        public bool? IsThumbnail { get; set; }
    }

    public class GetDonationImageDTO
    {
        public int ImageId { get; set; }

        public int? DonationId { get; set; }

        public string? Url { get; set; }

        public bool? IsThumbnail { get; set; }
    }
}
