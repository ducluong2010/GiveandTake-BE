using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveandTake_Repo.DTOs.Account
{
    public class AuthenDTO
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string Token { get; set; }
    }
}
