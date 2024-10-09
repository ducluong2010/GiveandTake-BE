using GiveandTake_Repo.Repository.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Business
{
    public class AuthenticationBusiness
    {
        private readonly UnitOfWork _unitOfWork;

        public AuthenticationBusiness()
        {
            _unitOfWork ??= new UnitOfWork();
        }

    }
}
