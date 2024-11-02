using Giveandtake_Services.Interfaces;
using MimeKit;
using MailKit.Net.Smtp;
using Giveandtake_Business;

namespace Giveandtake_Services.Implements
{
    public class EmailService : IEmailService
    {
        private readonly EmailBusiness _emailBusiness;

        public EmailService()
        {
            _emailBusiness = new EmailBusiness();
        }

        public async Task SendVerificationEmail(string email)
        {
            if (await _emailBusiness.IsAccountActive(email))
            {
                throw new Exception("Tài khoản đã được kích hoạt, không cần gửi mã xác nhận.");
            }

            var otp = _emailBusiness.GenerateOtp();
            var hashedOtp = _emailBusiness.HashOtp(otp);
            await _emailBusiness.UpdateOtp(email, hashedOtp);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Give&Take", "givetakesharing@gmail.com"));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = "Xác nhận tài khoản của bạn";

            message.Body = new TextPart("html") 
            { Text = $@"
                <!DOCTYPE html>
                <html lang='vi'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Xác Nhận Địa Chỉ Email</title>
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
                            <h1>Xác nhận địa chỉ email của bạn</h1>
                        </div>
                        <div class='email-content'>
                            <p>Xin chào {email},</p>
                            <p>Cảm ơn bạn đã đăng ký tài khoản Give&Take. Chỉ cần nhập mã OTP bên dưới để xác minh địa chỉ email của bạn và bắt đầu.</p>
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

        public async Task<string> ConfirmOtp(string email, string otp)
        {
            return await _emailBusiness.ConfirmOtp(email, otp);
        }
    }
}