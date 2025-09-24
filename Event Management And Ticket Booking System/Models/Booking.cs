using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Event_Management_And_Ticket_Booking_System.Models
{
    public enum BookingStatus { Pending = 0, Confirmed = 1, Cancelled = 2, Expired = 3 }

    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        [ForeignKey("User")]
        [ValidateNever]
        public string? UserId { get; set; }
        public IdentityUser User { get; set; } = null!;
        [ForeignKey("Event")]
        [ValidateNever]
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;

        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public BookingStatus BookStatus { get; set; } = BookingStatus.Pending;

        public DateTime? ReservationExpiresUtc { get; set; }
        public string? InvoicePath { get; set; }
        public string? TicketToken { get; set; } 


    }
}
