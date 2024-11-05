using GiveandTake_Repo.DTOs.Member;
using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Business
{
    public class MemberShipBusiness
    {
        private UnitOfWork _unitOfWork;

        public MemberShipBusiness()
        {
            _unitOfWork = new UnitOfWork();
        }

        public async Task UpdateMembershipAsync(MemberDTO updateDto)
        {
            var membership = await _unitOfWork.GetRepository<Membership>()
                 .SingleOrDefaultAsync(m => m.AccountId == updateDto.AccountId, null, null);

            if (membership == null)
            {
                membership = new Membership
                {
                    AccountId = updateDto.AccountId,
                    PurchaseDate = updateDto.PurchaseDate,
                    PremiumUntil = updateDto.PremiumUntil,
                    Status = updateDto.Status
                };

                await _unitOfWork.GetRepository<Membership>().InsertAsync(membership);
            }
            else
            {
                membership.PurchaseDate = updateDto.PurchaseDate;
                membership.PremiumUntil = updateDto.PremiumUntil;
                membership.Status = updateDto.Status;

                _unitOfWork.GetRepository<Membership>().UpdateAsync(membership);
            }

            await _unitOfWork.CommitAsync();
        }
    }
}
