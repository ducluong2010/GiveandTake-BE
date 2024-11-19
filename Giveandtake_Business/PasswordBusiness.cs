using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Business
{
    public class PasswordBusiness
    {
        private readonly UnitOfWork _unitOfWork;

        public PasswordBusiness()
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
                .SingleOrDefaultAsync(predicate: a => a.Email == email && a.IsActive == true);

            if (currentAcc == null)
            {
                throw new Exception("Email không tồn tại hoặc tài khoản chưa được kích hoạt");
            }

            currentAcc.Otp = hashedOtp;

            _unitOfWork.GetRepository<Account>().UpdateAsync(currentAcc);
            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> IsEmailRegisteredAndActive(string email)
        {
            Account currentAcc = await _unitOfWork.GetRepository<Account>()
                .SingleOrDefaultAsync(predicate: a => a.Email == email && a.IsActive == true);

            return currentAcc != null; 
        }

        public async Task<bool> ConfirmOtp(string email, string otp)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(otp))
            {
                throw new ArgumentException("Email và OTP không được để trống.");
            }

            var account = await _unitOfWork.GetRepository<Account>()
                .SingleOrDefaultAsync(predicate: a => a.Email == email && a.IsActive == true);

            if (account == null)
            {
                throw new Exception("Không tìm thấy tài khoản đã kích hoạt với email này.");
            }

            string hashedOtp = HashOtp(otp);  

            if (account.Otp != hashedOtp)
            {
                throw new Exception("OTP không hợp lệ.");
            }

            return true;
        }

        public async Task ChangePassword(string email, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                throw new ArgumentException("Tất cả các trường đều không được để trống.");
            }

            var account = await _unitOfWork.GetRepository<Account>()
              .SingleOrDefaultAsync(predicate: a => a.Email == email && a.IsActive == true);

            if (account == null)
            {
                throw new Exception("Không tìm thấy tài khoản với email này.");
            }

            if (newPassword != confirmPassword)
            {
                throw new Exception("Mật khẩu mới và mật khẩu xác nhận không khớp.");
            }

            if (newPassword.Length < 6)
            {
                throw new Exception("Mật khẩu mới phải có ít nhất 6 ký tự.");
            }

            account.Password = newPassword;
            account.Otp = null;

            _unitOfWork.GetRepository<Account>().UpdateAsync(account);
            await _unitOfWork.CommitAsync();
        }

    }
}
