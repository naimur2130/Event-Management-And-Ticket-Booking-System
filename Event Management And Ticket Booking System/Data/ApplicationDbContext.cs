using Event_Management_And_Ticket_Booking_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Event_Management_And_Ticket_Booking_System.Data
{
    public class ApplicationDbContext : IdentityDbContext 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Event> Event { get; set; }
        public DbSet<EventCategory> EventCategory { get; set; }
        public DbSet<Booking> Booking { get; set; }
        public DbSet<Tickets> Tickets { get; set; }
        public DbSet<TempAttendees> TempAttendees { get; set; } 
        public DbSet<EventPhoto> EventPhoto { get; set; }
        public DbSet<Payment> Payment { get; set; } 
        public DbSet<UserProfile> UserProfile { get; set; }
        public DbSet<SubscriptionPayment> SubscriptionPayment { get; set; }

        }
}
