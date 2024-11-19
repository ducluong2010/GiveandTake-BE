using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Interfaces
{
    public interface IPasswordService
    {
        Task SendPasswordResetOtp(string email);
        Task<bool> ConfirmOtp(string email, string otp);
        Task ChangePassword(string email, string newPassword, string confirmPassword);
    }
}
