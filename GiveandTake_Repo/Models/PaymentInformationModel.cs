using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveandTake_Repo.Models
{
    public class PaymentInformationModel
    {
        [Required(ErrorMessage = "AccountId is required")]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "OrderType is required")]
        public string OrderType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount is required")]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "OrderDescription is required")]
        public string OrderDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;
    }
}
