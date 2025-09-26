using Event_Management_And_Ticket_Booking_System.Data;
using Event_Management_And_Ticket_Booking_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Event_Management_And_Ticket_Booking_System.Areas.AttendeeArea.Controllers
{
    [Area("AttendeeArea")]
    [Authorize(Roles = "Attendee")]
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _web;

        public EventController(ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IWebHostEnvironment web)
        {
            _context = context;
            _userManager = userManager;
            _web = web;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var events = await _context.Event
                .Include(u => u.EventCategory)
                .Where(e => e.CreatedByType == EventCreatedByType.Attendee && e.Status==EventStatus.Published)
                .ToListAsync();

            return View(events);
        }

        public async Task<IActionResult> ShowEvents()
        {
            var events = await _context.Event
                .Include(u => u.EventCategory)
                .Where(e => e.Status==EventStatus.Draft)
                .ToListAsync();
            return View(events);
        }
        public IActionResult AddEvents()
        {
            ViewBag.CategoryList = _context.EventCategory
                .Select(u => new SelectListItem
                {
                    Value = u.CategoryId.ToString(),
                    Text = u.CategoryName
                }).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEvents(Event events, IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryList = _context.EventCategory.Select(u => new SelectListItem
                {
                    Value = u.CategoryId.ToString(),
                    Text = u.CategoryName
                }).ToList();
                return View(events);
            }

            if (file == null)
            {
                ModelState.AddModelError("BannerImagePath", "Banner image is required.");
                return View(events);
            }
            string wwwRootPath = _web.WebRootPath;
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string imagePath = Path.Combine(wwwRootPath, @"images\Event");

            if (!Directory.Exists(imagePath))
                Directory.CreateDirectory(imagePath);

            using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var user = await _userManager.GetUserAsync(User);

            events.UserId = user.Id;
            events.BannerImagePath = @"\images\Event\" + fileName;
            events.CreatedAt = DateTime.UtcNow;
            events.UpdatedAt = null;
            events.Status = EventStatus.PendingApproval;
            events.IsDeleted = false;
            events.CreatedByType = EventCreatedByType.Attendee;

            _context.Event.Add(events);
            await _context.SaveChangesAsync();

            TempData["success"] = "Your event request has been submitted for admin approval.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateEvent(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var events = await _context.Event.FirstOrDefaultAsync(u => u.EventId == id && u.UserId == user.Id);

            if (events == null) return NotFound();

            ViewBag.CategoryList = _context.EventCategory.Select(u => new SelectListItem
            {
                Value = u.CategoryId.ToString(),
                Text = u.CategoryName
            }).ToList();

            return View(events);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEvent(Event events, IFormFile? file)
        {
            var user = await _userManager.GetUserAsync(User);
            var existingEvent = await _context.Event.FirstOrDefaultAsync(u => u.EventId == events.EventId && u.UserId == user.Id);

            if (existingEvent == null) return NotFound();

            existingEvent.Title = events.Title;
            existingEvent.EventDescription = events.EventDescription;
            existingEvent.CategoryId = events.CategoryId;
            existingEvent.EventLocation = events.EventLocation;
            existingEvent.EventStartUtc = events.EventStartUtc;
            existingEvent.EventEndUtc = events.EventEndUtc;
            existingEvent.UpdatedAt = DateTime.UtcNow;

            if (file != null)
            {
                string wwwRootPath = _web.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string imagePath = Path.Combine(wwwRootPath, @"images\Event");

                if (!Directory.Exists(imagePath))
                {
                    Directory.CreateDirectory(imagePath);
                }

                if (!string.IsNullOrEmpty(existingEvent.BannerImagePath))
                {
                    var oldImagePath = Path.Combine(wwwRootPath, existingEvent.BannerImagePath.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }

                using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                existingEvent.BannerImagePath = @"\images\Event\" + fileName;
            }
            existingEvent.Status = EventStatus.Published;

            _context.Event.Update(existingEvent);
            await _context.SaveChangesAsync();

            TempData["success"] = "Event updated successfully. Waiting for admin approval again.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var events = await _context.Event
                .Include(u => u.EventCategory)
                .FirstOrDefaultAsync(u => u.EventId == id && u.UserId == user.Id);

            if (events == null) return NotFound();

            return View(events);
        }

        [HttpPost, ActionName("DeleteConfirm")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int EventId)
        {
            var user = await _userManager.GetUserAsync(User);
            var events = await _context.Event.FirstOrDefaultAsync(u => u.EventId == EventId && u.UserId == user.Id);

            if (events == null) return NotFound();

            events.IsDeleted = true;
            _context.Event.Update(events);
            await _context.SaveChangesAsync();

            TempData["success"] = "Event deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
