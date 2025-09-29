using System.ComponentModel.DataAnnotations;

namespace Event_Management_And_Ticket_Booking_System.ViewModel
{
    public class TicketViewModel
    {
        public int EventId { get; set; }
        public string EventTitle { get; set; }

        [Required]
        [Range(1, 10, ErrorMessage = "You can book 1-10 tickets at a time.")]
        public int Quantity { get; set; } = 1;
        public List<AttendeeForm> Attendees { get; set; } = new();
    }
}
