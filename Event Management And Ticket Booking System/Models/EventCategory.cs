using System.ComponentModel.DataAnnotations;

namespace Event_Management_And_Ticket_Booking_System.Models
{
    public class EventCategory
    {
        [Key]
        public int CategoryId { get; set; } 
        [Required]
        public string CategoryName { get; set; } = null!;
        public string CategoryDescription { get; set; } = null!;
    }
}
