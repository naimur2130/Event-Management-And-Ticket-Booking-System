using Event_Management_And_Ticket_Booking_System.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Tickets
{
    [Key]
    public int TicketId { get; set; }

    [ForeignKey("Booking")]
    public int BookingId { get; set; }
    [ValidateNever]
    public Booking? Booking { get; set; }

    [Required]
    public string AttendeeName { get; set; }

    [Required, EmailAddress]
    public string AttendeeEmail { get; set; }

    public string AttendeePhone { get; set; }

    [Required]
    public string TicketCode { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
