using Giveandtake_Business;
using Giveandtake_Services.Interfaces;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using MailKit.Net.Smtp;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Implements
{
    public class PasswordService : IPasswordService
    {
        private readonly PasswordBusiness _passwordBusiness;

        public PasswordService()
        {
            _passwordBusiness = new PasswordBusiness();
        }

        public async Task SendPasswordResetOtp(string email)
        {
            if (!await _passwordBusiness.IsEmailRegisteredAndActive(email))
            {
                throw new Exception("Email không tồn tại hoặc tài khoản chưa được kích hoạt.");
            }

            var otp = _passwordBusiness.GenerateOtp(6); 
            var hashedOtp = _passwordBusiness.HashOtp(otp);

            await _passwordBusiness.UpdateOtp(email, hashedOtp);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Give&Take", "givetakesharing@gmail.com"));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = "Đặt lại mật khẩu của bạn";

            message.Body = new TextPart("html")
            {
                Text = $@"
                <!DOCTYPE html>
                <html lang='vi'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Xác nhận quên mật khẩu</title>
                    <style>
                        body {{ 
                            font-family: 'Helvetica', Arial, sans-serif; 
                            background-color: #faf5ff; 
                            margin: 0; 
                            padding: 0; 
                            line-height: 1.5; 
                            color: #1e293b;
                        }}
                        .email-container {{ 
                            max-width: 600px; 
                            margin: 0 auto; 
                            background-color: #fff; 
                        }}
                        .email-header {{ 
                            background-color: #eb4747; 
                            color: white; 
                            text-align: center; 
                            padding: 20px; 
                        }}
                        .email-content {{ 
                            padding: 30px; 
                            text-align: center; 
                        }}
                        .otp-code {{ 
                            display: inline-block;
                            background-color: #eb4747; 
                            color: white; 
                            padding: 12px 24px; 
                            border-radius: 4px; 
                            font-size: 16px; 
                            font-weight: bold; 
                            margin: 20px 0; 
                            letter-spacing: 1px;
                        }}
                        .footer {{ 
                            text-align: center; 
                            padding: 20px; 
                            font-size: 14px; 
                            color: #444; 
                            border-top: 1px solid #eb4747;
                        }}
                        .address {{ 
                            margin-top: 10px; 
                            font-size: 12px; 
                        }}
                    </style>
                </head>
                <body>
                    <div class='email-container'>
                        <div class='email-header'>
                            <h1>Xác nhận đặt lại mật khẩu qua mail</h1>
                        </div>
                        <div class='email-content'>
                            <p>Xin chào {email},</p>
                            <p>Chúng tôi đã nhận được yêu cầu quên mật khẩu của bạn. Chỉ cần nhập mã OTP bên dưới để đổi mật khẩu mới của bạn và bắt đầu.</p>
                            <p>Vui lòng không tiết lộ mã cho bất kỳ ai để đảm bảo an toàn cho tài khoản của bạn.</p>
                            <div class='otp-code'>{otp}</div>
                        </div>
                        <div class='footer'>
                            <p>Đường D1, Đ. D1, Long Thạnh Mỹ, Thành Phố Thủ Đức, Hồ Chí Minh</p>
                            <div class='address'>
                                <strong>| Chính sách bảo mật | Thông tin liên hệ |</strong>
                            </div>
                        </div>
                    </div>
                </body>
                </html>
                "
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("givetakesharing@gmail.com", "edrw bnlk tkfr gipj");
                client.Send(message);
                client.Disconnect(true);
            }
        }
        public async Task<bool> ConfirmOtp(string email, string otp)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(otp))
            {
                throw new ArgumentException("Email và OTP không được để trống.");
            }

            return await _passwordBusiness.ConfirmOtp(email, otp);
        }

        public async Task ChangePassword(string email, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                throw new ArgumentException("Tất cả các trường đều không được để trống.");
            }

            await _passwordBusiness.ChangePassword(email, newPassword, confirmPassword);
        }
    }
}
