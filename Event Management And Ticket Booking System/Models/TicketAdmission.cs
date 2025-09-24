using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Event_Management_And_Ticket_Booking_System.Models
{
    public class TicketAdmission
    {
        [Key]
        public int TicketAdmissionId { get; set; }
        [ForeignKey("Booking")]
        [ValidateNever]
        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;
        public DateTime ScannedAt { get; set; } = DateTime.UtcNow;
        [ForeignKey("ScannedByUser")]
        [ValidateNever]
        public string? ScannedByUserId { get; set; }
        public IdentityUser? ScannedByUser { get; set; }
        public string ScanResult { get; set; } = "OK";
    }
}
