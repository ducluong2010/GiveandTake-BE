using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveandTake_Repo.DTOs.Request
{
    public class GetRequestDTO
    {
        public int RequestId { get; set; }

        public int? AccountId { get; set; }

        public int? DonationId { get; set; }

        public DateTime? RequestDate { get; set; }

        public string? Status { get; set; }
    }

    public class RequestDTO
    {
        public int? AccountId { get; set; }

        public int? DonationId { get; set; }

        public DateTime? RequestDate { get; set; }

        public string? Status { get; set; }
    }
}
