using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveandTake_Repo.DTOs.Member
{
    public class MemberDTO
    {
        public int AccountId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime PremiumUntil { get; set; }
        public string Status { get; set; }
    }
}
