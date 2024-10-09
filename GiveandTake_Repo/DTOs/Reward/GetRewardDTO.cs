using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveandTake_Repo.DTOs.Reward
{
    public class GetRewardDTO
    {
        public int RewardId { get; set; }

        public int? AccountId { get; set; }

        public string? RewardName { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public int? Point { get; set; }

        public int? Quantity { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string? Status { get; set; }
    }

    public class RewardDTO
    {
        public string? RewardName { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public int? Point { get; set; }

        public int? Quantity { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string? Status { get; set; }
    }
}
