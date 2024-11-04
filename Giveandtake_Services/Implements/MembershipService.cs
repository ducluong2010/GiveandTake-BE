using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Member;
using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using Giveandtake_Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace Giveandtake_Services.Implements
{
    public class MembershipService : IMembershipService
    {
        private readonly IConfiguration _configuration;
        private readonly string _hashSecret;
        private readonly UnitOfWork _unitOfWork;
        public MembershipService(IConfiguration configuration)
        {
            _configuration = configuration;
            _hashSecret = configuration["Vnpay:HashSecret"] ?? throw new InvalidOperationException("Vnpay:HashSecret configuration is required");
            _unitOfWork ??= new UnitOfWork();
        }
        public string CreatePaymentUrlAsync(PaymentInformationModel model, HttpContext context)
        {
            try
            {
                var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"] ?? "SE Asia Standard Time");
                var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
                var tick = DateTime.Now.Ticks.ToString();
                var pay = new VnPayLibrary();
                var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];
                if (string.IsNullOrEmpty(urlCallBack))
                    throw new InvalidOperationException("PaymentCallBack:ReturnUrl configuration is required");
                pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
                pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
                pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
                pay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString());
                pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
                pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
                pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
                pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
                pay.AddRequestData("vnp_OrderInfo", $"{model.Name} {model.OrderDescription} {model.Amount} AccountId={model.AccountId}");
                pay.AddRequestData("vnp_OrderType", model.OrderType);
                pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
                pay.AddRequestData("vnp_TxnRef", tick);
                var paymentUrl = pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _hashSecret);
                return paymentUrl;
            }
            catch (Exception ex)
            {
                // Log error
                throw new Exception("Error creating payment URL", ex);
            }
        }
        public PaymentResponseModel PaymentExecuteAsync(IQueryCollection collections)
        {
            if (collections == null || !collections.Any())
            {
                return new PaymentResponseModel { Success = false };
            }

            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _hashSecret);

            var orderInfoValue = collections["vnp_OrderInfo"].ToString(); 
            var accountIdString = orderInfoValue.Split("AccountId=").LastOrDefault();

            int accountId = 0;
            if (int.TryParse(accountIdString, out int parsedId))
            {
                accountId = parsedId;
            }

            response.AccountId = accountId; 
            return response;
        }


        public async Task UpdateAccountIsPremiumAsync(int accountId)
        {
            var account = await _unitOfWork.GetRepository<Account>()
               .SingleOrDefaultAsync(
                   predicate: a => a.AccountId == accountId,
                   orderBy: null,
                   include: null
               );

            if (account != null)
            {
                account.IsPremium = true;

                _unitOfWork.GetRepository<Account>().UpdateAsync(account);
                await _unitOfWork.CommitAsync();
            }
        }

        public async Task<PaymentResponseModel> HandlePaymentCallbackAsync(IQueryCollection collections)
        {
            if (collections == null || !collections.Any())
            {
                return new PaymentResponseModel { Success = false };
            }

            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _hashSecret);

            var orderInfoValue = collections["vnp_OrderInfo"].ToString();
            var accountIdString = orderInfoValue.Split("AccountId=").LastOrDefault();

            int accountId = 0;
            if (int.TryParse(accountIdString, out int parsedId))
            {
                accountId = parsedId;

                var memberShipBusiness = new MemberShipBusiness();

                var purchaseDate = DateTime.UtcNow; 
                var premiumUntil = purchaseDate.AddMonths(1); 

                var newMemberDto = new MemberDTO
                {
                    AccountId = accountId,
                    PurchaseDate = purchaseDate,
                    PremiumUntil = premiumUntil,
                    Status = "true" 
                };

                await memberShipBusiness.UpdateMembershipAsync(newMemberDto); 
            }

            response.AccountId = accountId;
            return response;
        }

    }
}