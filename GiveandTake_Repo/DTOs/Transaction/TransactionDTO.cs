using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveandTake_Repo.DTOs.Transaction
{
    public class TransactionDTO
    {
        public class CreateTransaction
        {
            public int? TotalPoint { get; set; }

            public DateTime? CreatedDate { get; set; }

            public DateTime? UpdatedDate { get; set; }

            public string? Status { get; set; }

            public int? AccountId { get; set; }
        }

        public class UpdateTransaction
        {
            public int? TotalPoint { get; set; }

            public DateTime? CreatedDate { get; set; }

            public DateTime? UpdatedDate { get; set; }

            public string? Status { get; set; }

            public int? AccountId { get; set; }
        }

        public class GetTransaction
        {
            public int TransactionId { get; set; }

            public int? TotalPoint { get; set; }

            public DateTime? CreatedDate { get; set; }

            public DateTime? UpdatedDate { get; set; }

            public string? Status { get; set; }

            public int? AccountId { get; set; }

            public bool? IsFeedback { get; set; }

        }
    }
}
