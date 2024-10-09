using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveandTake_Repo.DTOs.Reward
{
    public class GetRewardedDTO
    {
        public int RewardedId { get; set; }

        public int? RewardId { get; set; }

        public int? AccountId { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }
    }

    public class RewardedDTO
    {
        public int? RewardId { get; set; }

        public int? AccountId { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
