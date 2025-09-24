using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Event_Management_And_Ticket_Booking_System.Models
{
    public enum PaymentStatus { Initiated = 0, Success = 1, Failed = 2, Refunded = 3 }

    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        [ForeignKey("Booking")]
        
        public int BookingId { get; set; }
        [ValidateNever]
        public Booking Booking { get; set; } = null!;
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        public string Provider { get; set; } = "Stripe";
        public string PaymentMethod { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "BDT";
        public string TransactionId { get; set; } = null!;
        public PaymentStatus PayStatus { get; set; } = PaymentStatus.Initiated;
        public string? RawPayload { get; set; } 
    }
}
