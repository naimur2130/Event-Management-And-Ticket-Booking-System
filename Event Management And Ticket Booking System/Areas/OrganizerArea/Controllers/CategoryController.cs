using Event_Management_And_Ticket_Booking_System.Data;
using Event_Management_And_Ticket_Booking_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Event_Management_And_Ticket_Booking_System.Areas.OrganizerArea.Controllers
{
    [Area("OrganizerArea")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<EventCategory> categories = _context.EventCategory.ToList();
            return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EventCategory category)
        {
            if (category == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return NotFound();
            }
            _context.EventCategory.Add(category);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.EventCategory.FirstOrDefault(u => u.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EventCategory category)
        {
            if (category == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return NotFound();
            }
            _context.EventCategory.Update(category);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var category = _context.EventCategory.FirstOrDefault(u => u.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("CategoryDelete")]
        public async Task<IActionResult> CategoryDelete(int CategoryId)
        {
            var category = await _context.EventCategory.FindAsync(CategoryId);
            if (category == null)
            {
                return NotFound();
            }
            _context.EventCategory.Remove(category);
            _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }

}
