using Event_Management_And_Ticket_Booking_System.Data;
using Event_Management_And_Ticket_Booking_System.Models;
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
        public ApprovalController(ApplicationDbContext context)
        {
            _context = context;
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
    }
}
