using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveandTake_Repo.DTOs.Transaction
{
    public class TransactionDetailDTO
    {
        public int? TransactionId { get; set; }

        public int? DonationId { get; set; }
    }

    public class GetTransactionDetailDTO
    {
        public int TransactionDetailId { get; set; }

        public int? TransactionId { get; set; }

        public int? DonationId { get; set; }
    }
}
