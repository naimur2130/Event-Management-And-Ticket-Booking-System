using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Sockets;

namespace Event_Management_And_Ticket_Booking_System.Models
{
    public enum EventStatus
    {
        Draft = 0,
        PendingApproval = 1,
        Published = 2,
        Cancelled = 3,
        Rejected = 4
    }
    public enum EventCreatedByType
    {
        Organizer = 0,
        Attendee = 1
    }
    public class Event
    {
        [Key]
        public int EventId { get; set; }
        [ForeignKey("User")]
        
        public string? UserId { get; set; }
        [ValidateNever]
        public IdentityUser User { get; set; } = null!;
        [Required]
        public string Title { get; set; } = null!;
        public string EventDescription { get; set; } = null!;
        [ForeignKey("EventCategory")]
       
        public int CategoryId { get; set; }
        [ValidateNever]
        public EventCategory EventCategory { get; set; } = null!;
        [Required]
        public string EventLocation { get; set; } = null!;
        [Required]
        public DateTime EventStartUtc { get; set; }
        [Required]
        public DateTime EventEndUtc { get; set; }
        public string? BannerImagePath { get; set; }
        public string? Slug { get; set; }
        [Required]
        public EventStatus Status { get; set; } = EventStatus.Draft;
        [Required]
        public EventCreatedByType CreatedByType { get; set; } = EventCreatedByType.Organizer;
        [Required]
        public int? MaxAttendees { get; set; }
        [Required]
        public int CurrentAttendees { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

    }
}
