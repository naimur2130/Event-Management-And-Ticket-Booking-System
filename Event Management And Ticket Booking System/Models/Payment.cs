using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Event_Management_And_Ticket_Booking_System.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        [ForeignKey("Booking")]
        public int BookingId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } 

        [Required]
        public bool IsSuccess { get; set; }

        public string TransactionId { get; set; } 

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        [ValidateNever]
        public Booking Booking { get; set; }
    }
}
