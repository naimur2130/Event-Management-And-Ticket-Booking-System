using Event_Management_And_Ticket_Booking_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Event_Management_And_Ticket_Booking_System.Data
{
    public class ApplicationDbContext : IdentityDbContext 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Booking> Booking { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<BookingDetail> BookingDetail { get; set; }
        public DbSet<TicketType> TicketType { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<TicketAdmission> TicketAdmission { get; set; }
        public DbSet<EventCategory> EventCategory { get; set; }

    }
}
