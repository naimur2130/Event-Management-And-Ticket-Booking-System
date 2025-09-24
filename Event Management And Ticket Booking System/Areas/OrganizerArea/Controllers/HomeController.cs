using Microsoft.AspNetCore.Mvc;

namespace Event_Management_And_Ticket_Booking_System.Areas.OrganizerArea.Controllers
{
    [Area("OrganizerArea")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
