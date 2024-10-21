using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Business
{
    public interface IGiveandtakeResult
    {
        int Status { get; set; }
        string Message { get; set; }
        object? Data { get; set; }
        string? Qrcode { get; set; }
    }

    public class  GiveandtakeResult : IGiveandtakeResult
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }
        public string? Qrcode { get; set; }

        public GiveandtakeResult()
        {
            Status = -1;
            Message = "Error";
        }

        public GiveandtakeResult(object? data)
        {
            Status = 1;
            Message = "Success";
            Data = data;
        }

        public GiveandtakeResult(int status, string message)
        {
            Status = status;
            Message = message;
        }

        public GiveandtakeResult(int status, string message, object? data)
        {
            Status = status;
            Message = message;
            Data = data;
        }
    }
}
