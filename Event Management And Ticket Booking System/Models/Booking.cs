using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Sockets;

namespace Event_Management_And_Ticket_Booking_System.Models
{
    public enum BookingStatus
    {
        Pending=0,
        Confirmed=1,
        Cancelled=2,
        Failed= 3,
        AwaitApproval = 4,
        Rejected = 5,
        None = 6
    }
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        [ForeignKey("Event")]
        public int EventId { get; set; }
        [ValidateNever]
        public Event? Event { get; set; }
        [ForeignKey("User")]
        public string? UserId { get; set; } 
        public IdentityUser? User { get; set; }
        [ForeignKey("UserProfile")]
        public int UserProfileId { get; set; }
        public UserProfile? UserProfile { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }
        [Required]
        public BookingStatus Status { get; set; } = BookingStatus.None;
        [Required]
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public ICollection<Tickets> Tickets { get; set; }
        public ICollection<TempAttendees> TempAttendees { get; set; } = new List<TempAttendees>();
    }
}
