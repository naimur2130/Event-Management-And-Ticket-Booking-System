using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Event_Management_And_Ticket_Booking_System.Models
{

    public enum SubscriptionStatus
    {
        AwaitApproval=0,
        PendingPayment=1, 
        Active=2, 
        Rejected=3,
        Expired=4,
        NonSubscribed= 5
    }
    public enum SubscriptionType
    {
        Monthly=0,
        Quarterly=1,
        Yearly=2
    }
    public class UserProfile
    {
        [Key]
        public int UserProfileId { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }   
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public bool IsSubscribed { get; set; } = false;
        public DateTime? SubscriptionStart { get; set; }
        public DateTime? SubscriptionExpiry { get; set; }
        public decimal? SubscriptionAmount { get; set; }

        public SubscriptionStatus SubscriptionStatus { get; set; } = SubscriptionStatus.NonSubscribed;
        public SubscriptionType? SubscriptionType { get; set; }
        public string Image { get; set; }
        public ICollection<Booking> Booking { get; set; }
        public ICollection<SubscriptionPayment> SubscriptionPayment { get; set; }
        
    }
}
