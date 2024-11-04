using Microsoft.AspNetCore.Mvc;
using GiveandTake_Repo.Models;
using Giveandtake_Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace GiveandTake_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembershipController : ControllerBase
    {
        private readonly IMembershipService _membershipService;

        public MembershipController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        [HttpPost("create-payment")]
        public IActionResult CreatePayment([FromBody] PaymentInformationModel model)
        {
            try
            {
                // Kiểm tra ModelState
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Message = "Thông tin thanh toán không hợp lệ", Errors = ModelState });
                }

                // Kiểm tra null và amount
                if (model == null || model.Amount <= 0)
                {
                    return BadRequest(new { Message = "Thông tin thanh toán không hợp lệ" });
                }

                var paymentUrl = _membershipService.CreatePaymentUrlAsync(model, HttpContext);

                // Kiểm tra paymentUrl không null
                if (string.IsNullOrEmpty(paymentUrl))
                {
                    return BadRequest(new { Message = "Không thể tạo URL thanh toán" });
                }

                return Ok(new
                {
                    PaymentUrl = paymentUrl,
                    Message = "Tạo URL thanh toán thành công"
                });
            }
            catch (Exception ex)
            {

                var errorDetails = new List<string>
        {
            $"Message: {ex.Message}",
            $"StackTrace: {ex.StackTrace}"
        };


                if (ex.InnerException != null)
                {
                    errorDetails.Add($"InnerException Message: {ex.InnerException.Message}");
                    errorDetails.Add($"InnerException StackTrace: {ex.InnerException.StackTrace}");
                }

                Console.WriteLine(string.Join(Environment.NewLine, errorDetails));

                return StatusCode(500, new
                {
                    Message = "Có lỗi xảy ra khi tạo URL thanh toán",
                    Details = errorDetails 
                });
            }
        }

        [HttpGet("payment-callback")]
        public async Task<IActionResult> PaymentCallback()
        {
            try
            {
                var response = _membershipService.PaymentExecuteAsync(Request.Query);

                if (response.Success)
                {
                    //var paymentResponse = await _membershipService.HandlePaymentCallbackAsync(Request.Query);
                    await _membershipService.UpdateAccountIsPremiumAsync(response.AccountId);

                    return Ok(new
                    {
                        Success = true,
                        Message = "Thanh toán thành công",
                        Data = new
                        {
                            OrderId = response.OrderId,
                            PaymentMethod = response.PaymentMethod,
                            PaymentId = response.PaymentId,
                            TransactionId = response.TransactionId,
                            OrderDescription = response.OrderDescription,
                            VnPayResponseCode = response.VnPayResponseCode
                        }
                    });
                }

                return BadRequest(new
                {
                    Success = false,
                    Message = "Thanh toán thất bại",
                    VnPayResponseCode = response.VnPayResponseCode
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi xử lý callback thanh toán",
                    Error = ex.Message
                });
            }
        }
    }
}