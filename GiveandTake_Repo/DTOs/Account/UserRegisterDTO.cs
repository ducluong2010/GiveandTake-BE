using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveandTake_Repo.DTOs.Account
{
    public class UserRegisterDTO
    {
        [Required]
        public string? FullName { get; set; }

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required]
        [Phone]
        public string? Phone { get; set; }

        public string? Address { get; set; }

    }
}
