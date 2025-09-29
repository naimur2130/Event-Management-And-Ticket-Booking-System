using Event_Management_And_Ticket_Booking_System.Data;
using Event_Management_And_Ticket_Booking_System.Models;
using Event_Management_And_Ticket_Booking_System.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Event_Management_And_Ticket_Booking_System.Areas.AttendeeArea.Controllers
{
    [Area("AttendeeArea")]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BookingController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public IActionResult SelectTickets(int eventId, int quantity)
        {
            if (quantity < 1) quantity = 1;
            return RedirectToAction("EnterAttendees", new { eventId, quantity });
        }

        [HttpGet]
        public async Task<IActionResult> EnterAttendees(int eventId, int quantity)
        {
            var ev = await _context.Event.FindAsync(eventId);
            if (ev == null) return NotFound();

            var model = new TicketViewModel
            {
                EventId = ev.EventId,
                EventTitle = ev.Title,
                Quantity = quantity,
                Attendees = new List<AttendeeForm>()
            };

            for (int i = 0; i < quantity; i++)
                model.Attendees.Add(new AttendeeForm());

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnterAttendees(TicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var ev = await _context.Event.FindAsync(model.EventId);
            if (ev == null) return NotFound();

            decimal totalAmount = ev.PricePerTicket * model.Quantity;

            var booking = new Booking
            {
                EventId = model.EventId,
                UserId = user.Id,
                Quantity = model.Quantity,
                TotalAmount = totalAmount,
                Status = BookingStatus.Pending
            };

            _context.Booking.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction("InitiatePayment", "Payment", new { area = "AttendeeArea", bookingId = booking.BookingId, attendeesJson = JsonConvert.SerializeObject(model.Attendees) });
        }


        public async Task<IActionResult> BookingSuccess(int bookingId)
        {
            var booking = await _context.Booking
                .Include(b => b.Tickets)
                .Include(b => b.Event)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null) return NotFound();
            return View(booking);
        }
    }
}
