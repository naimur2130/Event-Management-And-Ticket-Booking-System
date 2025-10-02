using System.ComponentModel.DataAnnotations;

namespace Event_Management_And_Ticket_Booking_System.Models
{
    public class EventPhoto
    {
        [Key]
        public int PhotoId { get; set; }
        public int EventId { get; set; }
        public string UploadedBy { get; set; }  
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadDateTime { get; set; }
    }
}
