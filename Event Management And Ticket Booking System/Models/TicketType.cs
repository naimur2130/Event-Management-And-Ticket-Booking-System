using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Event_Management_And_Ticket_Booking_System.Models
{
    public class TicketType
    {
        [Key]
        public int TicketTypeId { get; set; }
        [ForeignKey("Event")]
        [ValidateNever]
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;
        public string TicketTypeName { get; set; } = null!; 
        public decimal TicketPrice { get; set; }
        public int TotalQuantity { get; set; }
        public int AvailableQuantity { get; set; }

        public DateTime? SalesStartUtc { get; set; }
        public DateTime? SalesEndUtc { get; set; }
        public bool IsActive { get; set; } = true;
        [Timestamp] public byte[]? RowVersion { get; set; }
    }
}
