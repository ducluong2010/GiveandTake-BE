using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Giveandtake_Business
{
     public class EmailBusiness
     {
         private readonly UnitOfWork _unitOfWork;

         public EmailBusiness()
         {
            _unitOfWork = new UnitOfWork();
         }
         private const string Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

         public string GenerateOtp(int length = 6)
         {
            var random = new char[length];
            using (var rng = new RNGCryptoServiceProvider())
            {
                var data = new byte[length];
                rng.GetBytes(data);
                for (int i = 0; i < length; i++)
                {
                   random[i] = Chars[data[i] % Chars.Length];
                }
            }
            return new string(random);
         }

         public string HashOtp(string otp)
         {
             using (var sha256 = SHA256.Create())
             {
                 var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(otp));
                 return Convert.ToBase64String(bytes);
             }
         }

         public async Task UpdateOtp(string email, string hashedOtp)
         {
            Account currentAcc = await _unitOfWork.GetRepository<Account>()
                    .SingleOrDefaultAsync(predicate: a => a.Email == email);

            if (currentAcc == null)
            {
               throw new Exception("Account not found"); 
            }

             currentAcc.Otp = hashedOtp;

             _unitOfWork.GetRepository<Account>().UpdateAsync(currentAcc);
             await _unitOfWork.CommitAsync();
         }

        public async Task<string> ConfirmOtp(string email, string otp)
        {
            Account currentAcc = await _unitOfWork.GetRepository<Account>()
                    .SingleOrDefaultAsync(predicate: a => a.Email == email);

            if (currentAcc == null)
            {
                return "Tài khoản không tồn tại";
            }

            if (currentAcc.IsActive == true) 
            {
                return "Tài khoản đã được kích hoạt";
            }

            string hashedOtp = HashOtp(otp);
            if (hashedOtp == currentAcc.Otp)
            {
                currentAcc.IsActive = true; 
                _unitOfWork.GetRepository<Account>().UpdateAsync(currentAcc);
                await _unitOfWork.CommitAsync();
                return "Kích hoạt tài khoản thành công";
            }
            else
            {
                return "Mã OTP không đúng";
            }
        }

        public async Task<bool> IsAccountActive(string email)
        {
            Account currentAcc = await _unitOfWork.GetRepository<Account>()
                    .SingleOrDefaultAsync(predicate: a => a.Email == email);

            return currentAcc?.IsActive ?? false; 
        }
    }
}
