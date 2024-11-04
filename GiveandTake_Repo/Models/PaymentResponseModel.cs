using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveandTake_Repo.Models
{
    public class PaymentResponseModel
    {
        public string OrderDescription { get; set; } = string.Empty;

        public string TransactionId { get; set; } = string.Empty;

        public int AccountId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string OrderId { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = string.Empty;

        public string PaymentId { get; set; } = string.Empty;

        public bool Success { get; set; }

        public string Token { get; set; } = string.Empty;

        public string VnPayResponseCode { get; set; } = string.Empty;

        public string GetResponseMessage()
        {
            return VnPayResponseCode switch
            {
                "00" => "Giao dịch thành công",
                "07" => "Giao dịch bị nghi ngờ (khách hàng có thể bị trừ tiền)",
                "09" => "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng chưa đăng ký dịch vụ InternetBanking tại ngân hàng",
                "10" => "Giao dịch không thành công do: Khách hàng xác thực thông tin thẻ/tài khoản không đúng quá 3 lần",
                "11" => "Giao dịch không thành công do: Đã hết hạn chờ thanh toán",
                "12" => "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng bị khóa",
                "13" => "Giao dịch không thành công do Quý khách nhập sai mật khẩu xác thực giao dịch (OTP)",
                "24" => "Giao dịch không thành công do: Khách hàng hủy giao dịch",
                "51" => "Giao dịch không thành công do: Tài khoản của quý khách không đủ số dư để thực hiện giao dịch",
                "65" => "Giao dịch không thành công do: Tài khoản của Quý khách đã vượt quá hạn mức giao dịch trong ngày",
                "75" => "Ngân hàng thanh toán đang bảo trì",
                "79" => "Giao dịch không thành công do: KH nhập sai mật khẩu thanh toán quá số lần quy định",
                _ => "Giao dịch thất bại"
            };
        }
    }
}
