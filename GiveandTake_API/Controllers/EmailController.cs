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
        public async Task<IActionResult> SendVerificationEmail([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email không được để trống");
            }

            try
            {
                await _emailService.SendVerificationEmail(email);
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
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Otp))
            {
                return BadRequest("Email và mã OTP không được để trống");
            }

            try
            {
                var result = await _emailService.ConfirmOtp(request.Email, request.Otp);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Đã có lỗi xảy ra: {ex.Message}");
            }
        }
    }

    public class ConfirmOtpRequest
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }
}
