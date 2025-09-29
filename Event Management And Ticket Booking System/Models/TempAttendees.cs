using System.ComponentModel.DataAnnotations;

namespace Event_Management_And_Ticket_Booking_System.Models
{
    public class TempAttendees
    {
        [Key]
        public int Id { get; set; }

        public int BookingId { get; set; }

        [Required]
        public string AttendeeName { get; set; }

        [Required]
        public string AttendeeEmail { get; set; }

        [Required]
        public string AttendeePhone { get; set; }
    }
}
