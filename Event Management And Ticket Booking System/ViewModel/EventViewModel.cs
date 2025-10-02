using Event_Management_And_Ticket_Booking_System.Models;

namespace Event_Management_And_Ticket_Booking_System.ViewModel
{
    public class EventViewModel
    {
        public IEnumerable<Event> OrganizerEvents { get; set; } = new List<Event>();
        public IEnumerable<Event> AttendeeEvents { get; set; } = new List<Event>();

        public string SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public List<EventCategory> Categories { get; set; }
        public string? Source { get; set; }
        public List<Booking> UserBookings { get; set; } = new List<Booking>();
        public List<AttendeeForm> Attendees { get; set; } = new List<AttendeeForm>();
    }
}
