using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Sockets;

namespace Event_Management_And_Ticket_Booking_System.Models
{
    public enum EventStatus { Draft = 0, Published = 1, Cancelled = 2 }
    public class Event
    {
        [Key]
        public int EventId { get; set; }
        [ForeignKey("User")]
        
        public string? UserId { get; set; }
        [ValidateNever]
        public IdentityUser User { get; set; } = null!;

        public string Title { get; set; } = null!;
        public string EventDescription { get; set; } = null!;
        [ForeignKey("EventCategory")]
       
        public int CategoryId { get; set; }
        [ValidateNever]
        public EventCategory EventCategory { get; set; } = null!;
        public string EventLocation { get; set; } = null!;
        public DateTime EventStartUtc { get; set; }
        public DateTime EventEndUtc { get; set; }
        public string? BannerImagePath { get; set; }
        public string? Slug { get; set; }

        public EventStatus Status { get; set; } = EventStatus.Draft;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

    }
}
