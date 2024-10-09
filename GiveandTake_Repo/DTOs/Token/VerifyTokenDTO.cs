using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveandTake_Repo.DTOs.Token
{
    public class VerifyTokenDTO
    {
        [Required]
        public string? Token { get; set; }

        public VerifyTokenDTO(string token)
        {
            Token = token;
        }
    }
}
