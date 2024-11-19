using Giveandtake_Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GiveandTake_API.Controllers
{
    public class ChangeController : ControllerBase
    {
        private readonly IPasswordService _passwordService;

        public ChangeController(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtpToChangePassword([FromBody] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Email không được để trống.");
            }

            try
            {
                await _passwordService.SendPasswordResetOtp(email);
                return Ok(new { message = "OTP đã được gửi đến email của bạn." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("confirm-otp-to-change")]
        public async Task<IActionResult> ConfirmOtp([FromBody] ChangeOtpRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Otp))
            {
                return BadRequest(new { message = "Email và OTP không được để trống." });
            }

            try
            {
                var isConfirmed = await _passwordService.ConfirmOtp(request.Email, request.Otp);
                if (isConfirmed)
                {
                    return Ok(new { message = "OTP hợp lệ." });
                }
                else
                {
                    return BadRequest(new { message = "OTP không hợp lệ." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                await _passwordService.ChangePassword(request.Email, request.NewPassword, request.ConfirmPassword);
                return Ok("Mật khẩu đã được thay đổi thành công.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public class ChangeOtpRequest
        {
            public string Email { get; set; }
            public string Otp { get; set; }
        }
        public class ChangePasswordRequest
        {
            public string Email { get; set; }
            public string NewPassword { get; set; }
            public string ConfirmPassword { get; set; }
        }
    }
}
