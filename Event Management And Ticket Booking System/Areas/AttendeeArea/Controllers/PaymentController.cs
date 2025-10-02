using Event_Management_And_Ticket_Booking_System.Data;
using Event_Management_And_Ticket_Booking_System.Models;
using Event_Management_And_Ticket_Booking_System.Services;
using Event_Management_And_Ticket_Booking_System.Services.IService;
using Event_Management_And_Ticket_Booking_System.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http;

namespace Event_Management_And_Ticket_Booking_System.Areas.AttendeeArea.Controllers
{
    [Area("AttendeeArea")]
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IEmailService _emailService;

        // SSLCommerz credentials
        private readonly string storeId = "iubat68daa0ff0a96f";
        private readonly string storePassword = "iubat68daa0ff0a96f@ssl";
        private readonly string sslCommerzApiUrl = "https://sandbox.sslcommerz.com/gwprocess/v4/api.php";

        // NGROK URL for testing
        private readonly string baseUrl = "https://badgeless-ariana-unstifled.ngrok-free.dev";

        public PaymentController(ApplicationDbContext context,
                                 UserManager<IdentityUser> userManager,
                                 IHttpClientFactory httpClientFactory,
                                 IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _httpClientFactory = httpClientFactory;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> InitiatePayment(int bookingId)
        {
            var booking = await _context.Booking
                                        .Include(b => b.Event)
                                        .Include(b => b.User)
                                        .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null) return NotFound();

            // Retrieve attendees
            var attendees = await _context.TempAttendees
                                          .Where(t => t.BookingId == bookingId)
                                          .ToListAsync();

            if (attendees == null || !attendees.Any())
                return Content("No attendee information found. Cannot proceed with payment.");

            // Prepare payment request
            var values = new Dictionary<string, string>
    {
        { "store_id", storeId },
        { "store_passwd", storePassword },
        { "total_amount", booking.TotalAmount.ToString() },
        { "currency", "BDT" },
        { "tran_id", $"EVT{booking.BookingId}{DateTime.Now.Ticks}" },
        { "success_url", $"{baseUrl}/AttendeeArea/Payment/PaymentSuccess?bookingId={booking.BookingId}" },
        { "fail_url", $"{baseUrl}/AttendeeArea/Payment/PaymentFailed?bookingId={booking.BookingId}" },
        { "cancel_url", $"{baseUrl}/AttendeeArea/Payment/PaymentCancelled?bookingId={booking.BookingId}" },
        { "cus_name", User.Identity.Name ?? "Guest" },
        { "cus_email", "example@example.com" },
        { "cus_add1", "N/A" },
        { "cus_city", "Dhaka" },
        { "cus_postcode", "1000" },
        { "cus_country", "Bangladesh" },
        { "cus_phone", "0123456789" },
        { "shipping_method", "NO" },
        { "product_name", booking.Event.Title },
        { "product_category", "Event" },
        { "product_profile", "general" }
    };

            var client = _httpClientFactory.CreateClient();
            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync(sslCommerzApiUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();

            dynamic? res = JsonConvert.DeserializeObject(responseString);
            string? redirectUrl = res?.GatewayPageURL;

            if (!string.IsNullOrEmpty(redirectUrl))
                return Redirect(redirectUrl);

            return Content("Unable to initiate payment. Please try again.");
        }


        [HttpGet]
        public async Task<IActionResult> PaymentSuccess(int bookingId)
        {
            var booking = await _context.Booking
                                        .Include(b => b.Event)
                                        .Include(b => b.User)
                                        .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null) return NotFound();

            booking.Status = BookingStatus.Confirmed;
            _context.Update(booking);
            await _context.SaveChangesAsync();

            // Retrieve temporary attendees
            var tempAttendees = await _context.TempAttendees
                                              .Where(t => t.BookingId == bookingId)
                                              .ToListAsync();

            var tickets = new List<Tickets>();
            foreach (var attendee in tempAttendees)
            {
                var ticket = new Tickets
                {
                    BookingId = booking.BookingId,
                    AttendeeName = attendee.AttendeeName,
                    AttendeeEmail = attendee.AttendeeEmail,
                    AttendeePhone = attendee.AttendeePhone,
                    TicketCode = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10).ToUpper()
                };
                _context.Tickets.Add(ticket);
                tickets.Add(ticket);
            }
            _context.TempAttendees.RemoveRange(tempAttendees);
            await _context.SaveChangesAsync();

            foreach (var ticket in tickets)
            {
                string emailBody = $@"
            <h3>Booking Confirmation</h3>
            <p>Dear {ticket.AttendeeName},</p>
            <p>Your booking for <strong>{booking.Event.Title}</strong> is confirmed.</p>
            <ul>
                <li><strong>Date:</strong> {booking.Event.EventStartUtc:g}</li>
                <li><strong>Location:</strong> {booking.Event.EventLocation}</li>
                <li><strong>Ticket Code:</strong> {ticket.TicketCode}</li>
            </ul>
            <p>Thank you for booking with us!</p>";

                await _emailService.SendEmailAsync(ticket.AttendeeEmail,
                                                   "Booking Confirmation - Event Ticket",
                                                   emailBody);
            }

            booking = await _context.Booking
                             .Include(b => b.Event)
                             .Include(b => b.Tickets)
                             .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            return View("~/Areas/AttendeeArea/Views/Booking/BookingSuccess.cshtml", booking);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentFailed(int bookingId)
        {
            var booking = await _context.Booking.FindAsync(bookingId);
            if (booking != null)
            {
                booking.Status = BookingStatus.Failed;
                _context.Update(booking);
                await _context.SaveChangesAsync();
            }

            return Content("Payment Failed. Please try again.");
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCancelled(int bookingId)
        {
            var booking = await _context.Booking.FindAsync(bookingId);
            if (booking != null)
            {
                booking.Status = BookingStatus.Cancelled;
                _context.Update(booking);
                await _context.SaveChangesAsync();
            }

            return Content("Payment Cancelled.");
        }
        [HttpGet]
        public async Task<IActionResult> DownloadTicket(int ticketId)
        {
            var ticket = await _context.Tickets
                                       .Include(t => t.Booking)
                                       .ThenInclude(b => b.Event)
                                       .FirstOrDefaultAsync(t => t.TicketId == ticketId);

            if (ticket == null) return NotFound();

            // Generate PDF for this single ticket
            var pdfBytes = TicketPdfGenerator.GeneratePdf(new List<Tickets> { ticket });

            return File(pdfBytes, "application/pdf", $"Ticket_{ticket.TicketCode}.pdf");
        }

    }
}
