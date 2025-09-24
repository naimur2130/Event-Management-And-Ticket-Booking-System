using Event_Management_And_Ticket_Booking_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Event_Management_And_Ticket_Booking_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                // Unauthenticated: show public home page (or redirect to login)
                return View();
                // return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // Session expired or user deleted
                return View();
            }

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return RedirectToAction("Index", "Home", new { area = "AdminArea" });
            }
            if (await _userManager.IsInRoleAsync(user, "Organizer"))
            {
                return RedirectToAction("Index", "Home", new { area = "OrganizerArea" });
            }
            if (await _userManager.IsInRoleAsync(user, "Attendee"))
            {
                return RedirectToAction("Index", "Home", new { area = "AttendeeArea" });
            }

            // If user has no recognized role
            return View("AccessDenied");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
