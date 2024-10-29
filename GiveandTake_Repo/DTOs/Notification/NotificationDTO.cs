using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveandTake_Repo.DTOs.Notification
{
    public class NotificationDTO
    {
        public int NotificationId { get; set; }
        public int? DonationId { get; set; }
        public string? DonationName { get; set; } 
        public int? AccountId { get; set; }
        public string? AccountName { get; set; } 
        public string? Note { get; set; }
        public string? Type { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? IsRead { get; set; }
        public int? StaffId { get; set; }
        public string? StaffName { get; set; } 
    }
    public class NotificationCreateDTO
    {
        public int? DonationId { get; set; }
        public int? AccountId { get; set; }
        public string? Note { get; set; }
        public string? Type { get; set; }
        public int? StaffId { get; set; }
    }

    public class NotificationUpdateDTO
    {
        public string? Note { get; set; }
        public string? Type { get; set; }
    }
}
