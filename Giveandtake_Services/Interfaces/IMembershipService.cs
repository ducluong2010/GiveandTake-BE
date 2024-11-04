using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GiveandTake_Repo.Models;
using Microsoft.AspNetCore.Http;

namespace Giveandtake_Services.Interfaces
{
    public interface IMembershipService
    {
        string CreatePaymentUrlAsync(PaymentInformationModel model, HttpContext context);
        PaymentResponseModel PaymentExecuteAsync(IQueryCollection collections);
        Task UpdateAccountIsPremiumAsync(int accountId);
        Task<PaymentResponseModel> HandlePaymentCallbackAsync(IQueryCollection collections);
    }
}
