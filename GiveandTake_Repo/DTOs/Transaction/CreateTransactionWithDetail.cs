using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveandTake_Repo.DTOs.Transaction
{
    public class CreateTransactionWithDetail
    {
        public TransactionDTO.CreateTransaction Transaction { get; set; }
        public TransactionDetailDTO TransactionDetail { get; set; }
    }
}
