using Event_Management_And_Ticket_Booking_System.Data;
using Event_Management_And_Ticket_Booking_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Event_Management_And_Ticket_Booking_System.Areas.OrganizerArea.Controllers
{
    [Area("OrganizerArea")]
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
            var events =await _context.Event.Include(u=>u.EventCategory).ToListAsync();
            return View(events);
        }

        public IActionResult AddEvents()
        {
            ViewBag.CategoryList = _context.EventCategory.Select(u => new SelectListItem
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
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return NotFound();
            }
            if(file==null) return NotFound();

            string wwwRootPath = _web.WebRootPath;
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string imagePath = Path.Combine(wwwRootPath, @"images\Event");
            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }
            using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            events.UserId = user.Id;
            events.BannerImagePath = @"\images\Event\" + fileName;
            events.CreatedAt = DateTime.UtcNow;
            events.Status = EventStatus.Draft;
            events.IsDeleted = false;
            events.UpdatedAt = null;
            _context.Event.Add(events);
            await _context.SaveChangesAsync();
            TempData["success"] = "Event created successfully";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> UpdateEvent(int id)
        {
            var events = await _context.Event.FirstOrDefaultAsync(u => u.EventId == id);
            if (events == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            if (events.UserId != user.Id)
            {
                return NotFound();
            }
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
            if (!ModelState.IsValid)
            {
                return NotFound();
            }
            var existingEvent = await _context.Event.AsNoTracking().FirstOrDefaultAsync(u => u.EventId == events.EventId);
            if (existingEvent == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            if (existingEvent.UserId != user.Id)
            {
                return NotFound();
            }
            if (file != null)
            {
                string wwwRootPath = _web.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string imagePath = Path.Combine(wwwRootPath, @"images\Event");
                if (!Directory.Exists(imagePath))
                {
                    Directory.CreateDirectory(imagePath);
                }
                var oldImagePath = Path.Combine(wwwRootPath, existingEvent.BannerImagePath!.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
                using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                existingEvent.BannerImagePath = @"\images\Event\" + fileName;
            }
            existingEvent.Title = events.Title;
            existingEvent.EventDescription = events.EventDescription;
            existingEvent.CategoryId = events.CategoryId;
            existingEvent.EventLocation = events.EventLocation;
            existingEvent.EventStartUtc = events.EventStartUtc;
            existingEvent.EventEndUtc = events.EventEndUtc;
            existingEvent.UpdatedAt = DateTime.UtcNow;
            
            _context.Event.Update(existingEvent);
            await _context.SaveChangesAsync();
            TempData["success"] = "Event updated successfully";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var events = _context.Event.Include(u => u.EventCategory).FirstOrDefault(u => u.EventId == id);
            return View(events);
        }

        [HttpPost, ActionName("DeleteConfirm")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirm(int EventId)
        {
            var events = _context.Event.FirstOrDefault(u => u.EventId == EventId);
            if (events == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(events.BannerImagePath))
            {
                string wwwRootPath = _web.WebRootPath;
                var oldPath = Path.Combine(wwwRootPath, events.BannerImagePath.TrimStart('\\'));
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }
            _context.Event.Remove(events);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
