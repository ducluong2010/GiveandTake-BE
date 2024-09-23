using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Business
{
    public class AccountBusiness
    {
        private readonly UnitOfWork _unitOfWork;

        public AccountBusiness()
        {
            _unitOfWork ??= new UnitOfWork();
        }

        #region Authentication
        // Method for user login
        public async Task<IGiveandtakeResult> Login(string email, string password)
        {
            Account account = await _unitOfWork.GetRepository<Account>()
                .SingleOrDefaultAsync(predicate: a => a.Email.Equals(email) && a.Password.Equals(password));

            GiveandtakeResult result = new GiveandtakeResult();

            if (account == null)
            {
                result.Status = -1;
                result.Message = "Incorrect Email or Password";
                return result;
            }

            if (!(bool)account.IsActive)
            {
                result.Status = -1;
                result.Message = "Your account is currently inactive (banned)";
                return result;
            }

            result.Data = JwtUtils.GenerateJwtToken(account);
            result.Status = 1;
            result.Message = "Login successfully";
            return result;
        }
    }
}