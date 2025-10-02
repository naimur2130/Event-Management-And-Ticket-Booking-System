using Event_Management_And_Ticket_Booking_System.Data;
using Event_Management_And_Ticket_Booking_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Event_Management_And_Ticket_Booking_System.Areas.AttendeeArea.Controllers
{
    [Area("AttendeeArea")]
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserProfileController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // View Profile
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var profile = await _context.UserProfile
                .FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (profile == null)
            {
                profile = new UserProfile
                {
                    UserId = user.Id,
                    Email = user.Email
                };
            }

            return View(profile);
        }

        [HttpGet]
        public IActionResult CreateProfile()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProfile(UserProfile model, IFormFile ImageFile)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            model.UserId = user.Id;
            model.Email = user.Email;

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ImageFile.FileName)}";
                var filePath = Path.Combine("wwwroot/uploads/profile", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                model.Image = "/uploads/profile/" + fileName;
            }

            _context.UserProfile.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Profile");
        }
        [HttpGet]
        public async Task<IActionResult> EditProfile(int id)
        {
            var profile = await _context.UserProfile.FindAsync(id);
            if (profile == null) return NotFound();

            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(UserProfile model, IFormFile ImageFile)
        {
            var profile = await _context.UserProfile.FindAsync(model.UserProfileId);
            if (profile == null) return NotFound();

            profile.FullName = model.FullName;
            profile.Phone = model.Phone;
            profile.Address = model.Address;
            profile.City = model.City;
            profile.Country = model.Country;

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ImageFile.FileName)}";
                var filePath = Path.Combine("wwwroot/uploads/profile", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                profile.Image = "/uploads/profile/" + fileName;
            }

            _context.Update(profile);
            await _context.SaveChangesAsync();

            return RedirectToAction("Profile");
        }
    }
}
