using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveandTake_Repo.DTOs.Donation
{
     public class DonationDTO
    {
        public int DonationId { get; set; }

        public int? AccountId { get; set; }

        public int? CategoryId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public int? Point { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? ApprovedBy { get; set; }

        public int? TotalRating { get; set; }

        public string? Status { get; set; }

        public List<string>? DonationImages { get; set; }
    }
    public class CreateUpdateDonationDTO
    {
        public int? AccountId { get; set; }

        public int? CategoryId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public int? Point { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? ApprovedBy { get; set; }

        public int? TotalRating { get; set; }

        public string? Status { get; set; }

        public List<string>? DonationImages { get; set; }
    }
    public class CreateDonationDTO
    {
        public int? AccountId { get; set; }

        public int? CategoryId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public int? Point { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? ApprovedBy { get; set; }

        public int? TotalRating { get; set; }

        public List<string>? DonationImages { get; set; }
    }
    public class DonationFilterDTO
    {
        public int? CategoryId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Point { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? ApprovedBy { get; set; }
        public string? Status { get; set; }
    }

}
