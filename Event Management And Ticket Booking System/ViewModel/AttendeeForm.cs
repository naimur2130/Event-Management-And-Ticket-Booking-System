using System.ComponentModel.DataAnnotations;

namespace Event_Management_And_Ticket_Booking_System.ViewModel
{
    public class AttendeeForm
    {
        [Required]
        public string AttendeeName { get; set; }

        [Required, EmailAddress]
        public string AttendeeEmail { get; set; }

        public string AttendeePhone { get; set; }
    }
}
