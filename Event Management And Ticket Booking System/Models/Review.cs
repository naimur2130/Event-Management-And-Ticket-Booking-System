using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Event_Management_And_Ticket_Booking_System.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }
        [ForeignKey("User")]
        [ValidateNever]
        public string? UserId { get; set; }
        public IdentityUser User { get; set; } = null!;
        [ForeignKey("Event")]
        [ValidateNever]
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;
        public int Rating { get; set; } 
        public string Comment { get; set; } = null!;
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

        public bool Approved { get; set; } = true;
        public string? OrganizerResponse { get; set; }
        public DateTime? RespondedAt { get; set; }
    }
}
