using System.ComponentModel.DataAnnotations;

namespace Event_Management_And_Ticket_Booking_System.Models
{
    public class SubscriptionPayment
    {
        [Key]
        public int SubscriptionPaymentId { get; set; }

        public int UserProfileId { get; set; }
        public UserProfile UserProfile { get; set; }

        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; }
        public string TransactionId { get; set; }
        public string PaymentMethod { get; set; }
    }
}
