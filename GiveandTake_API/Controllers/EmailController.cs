using Giveandtake_Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Giveandtake_Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text;
namespace GiveandTake_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send-verification")]
        public async Task<IActionResult> SendVerificationEmail([FromBody] SendVerificationRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest("Email không được để trống");
            }

            try
            {
                await _emailService.SendVerificationEmail(request.AccountId, request.Email);
                return Ok("Email xác minh đã được gửi");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Đã có lỗi xảy ra: {ex.Message}");
            }
        }

        [HttpPost("confirm-otp")]
        public async Task<IActionResult> ConfirmOtp([FromBody] ConfirmOtpRequest request)
        {
            if (request.AccountId <= 0 || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Otp))
            {
                return BadRequest("AccountId, Email, và mã OTP không được để trống");
            }

            try
            {
                var result = await _emailService.ConfirmOtp(request.AccountId, request.Email, request.Otp);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Đã có lỗi xảy ra: {ex.Message}");
            }
        }
    }

    public class SendVerificationRequest
    {
        public int AccountId { get; set; }
        public string Email { get; set; }
    }

    public class ConfirmOtpRequest
    {
        public int AccountId { get; set; }
        public string Email { get; set; }
        public string Otp { get; set; }
    }
}
