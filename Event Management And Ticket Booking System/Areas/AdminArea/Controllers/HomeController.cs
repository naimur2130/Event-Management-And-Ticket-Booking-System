using Microsoft.AspNetCore.Mvc;

namespace Event_Management_And_Ticket_Booking_System.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
