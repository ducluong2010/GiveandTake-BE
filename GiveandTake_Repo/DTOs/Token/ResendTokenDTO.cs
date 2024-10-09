using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveandTake_Repo.DTOs.Token
{
    public class ResendTokenDTO
    {
        [Required]
        public string? Email { get; set; }

        public ResendTokenDTO(string email)
        {
            Email = email;
        }
    }
}
