using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Event_Management_And_Ticket_Booking_System.Models
{
    public class BookingDetail
    {
        [Key]
        public int BookingDetailId { get; set; }
        [ForeignKey("Booking")]
        [ValidateNever]
        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;
        [ValidateNever]
        public int TicketTypeId { get; set; }
        public TicketType TicketType { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal PricePerTicket { get; set; }
    }
}
