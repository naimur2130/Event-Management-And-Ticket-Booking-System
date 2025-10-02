using Event_Management_And_Ticket_Booking_System.Data;
using Event_Management_And_Ticket_Booking_System.Models;
using Event_Management_And_Ticket_Booking_System.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Event_Management_And_Ticket_Booking_System.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    [Authorize(Roles = "Admin")]
    public class ApprovalController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        public ApprovalController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        public async Task<IActionResult> Index()
        {
            var requests = await _context.Event.Include(u => u.EventCategory).Include(u => u.User)
                .Where(u => u.Status == EventStatus.PendingApproval).ToListAsync();
            return View(requests);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var eventRequest = await _context.Event.FindAsync(id);
            if (eventRequest == null || eventRequest.Status != EventStatus.PendingApproval)
            {
                return NotFound();
            }
            eventRequest.Status = EventStatus.Published;
            eventRequest.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            TempData["success"] = "Event approved successfully!";
            return RedirectToAction("Index");

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var eventRequest = await _context.Event.FindAsync(id);
            if (eventRequest == null || eventRequest.Status != EventStatus.PendingApproval)
            {
                return NotFound();
            }
            eventRequest.Status = EventStatus.Rejected;
            eventRequest.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            TempData["success"] = "Event rejected successfully!";
            return RedirectToAction("Index");
        }

        public IActionResult PendingBookings()
        {
            var bookings = _context.Booking
                .Include(b => b.User)
                .Include(b => b.Event)
                .Where(b => b.Status == BookingStatus.AwaitApproval)
                .ToList();

            return View(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveBooking(int bookingId)
        {
            var booking = await _context.Booking
                                        .Include(b => b.Event)
                                        .Include(b => b.TempAttendees) 
                                        .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null) return NotFound();

            booking.Status = BookingStatus.Pending;
            await _context.SaveChangesAsync();

            foreach (var attendee in booking.TempAttendees)
            {
                string emailBody = $@"
            <h3>Booking Approved</h3>
            <p>Dear {attendee.AttendeeName},</p>
            <p>Your booking for the event <strong>{booking.Event.Title}</strong> has been approved by the admin.</p>
            <p>Please proceed with payment to confirm your booking.</p>
        ";

                await _emailService.SendEmailAsync(
                    attendee.AttendeeEmail,
                    "Your Event Booking is Approved",
                    emailBody
                );
            }

            return RedirectToAction("PendingBookings");
        }

        [HttpPost]
        public async Task<IActionResult> RejectBooking(int bookingId, string? reason = null)
        {
            var booking = await _context.Booking
                                        .Include(b => b.Event)
                                        .Include(b => b.TempAttendees)
                                        .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null) return NotFound();

            booking.Status = BookingStatus.Rejected;
            await _context.SaveChangesAsync();

            foreach (var attendee in booking.TempAttendees)
            {
                string emailBody = $@"
            <h3>Booking Rejected</h3>
            <p>Dear {attendee.AttendeeName},</p>
            <p>We are sorry to inform you that your booking for the event <strong>{booking.Event.Title}</strong> has been rejected.</p>
            {(string.IsNullOrEmpty(reason) ? "" : $"<p>Reason: {reason}</p>")}
            <p>Please contact support if you have any questions.</p>
        ";

                await _emailService.SendEmailAsync(
                    attendee.AttendeeEmail,
                    "Your Event Booking is Rejected",
                    emailBody
                );
            }

            return RedirectToAction("PendingBookings");
        }


    }
}
