using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Interfaces
{
    public interface IEmailService
    {
        Task SendVerificationEmail(int accountId, string email);
        Task<string> ConfirmOtp(int accountId, string email, string otp);
    }
}
