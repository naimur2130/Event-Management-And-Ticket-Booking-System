using Event_Management_And_Ticket_Booking_System.Data;
using Event_Management_And_Ticket_Booking_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Event_Management_And_Ticket_Booking_System.Areas.AttendeeArea.Controllers
{
    [Area("AttendeeArea")]
    public class SubscriptionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string baseUrl = "https://badgeless-ariana-unstifled.ngrok-free.dev";
        public SubscriptionController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Subscribe(int id)
        {
            var profile = await _context.UserProfile.FindAsync(id);
            if (profile == null) return NotFound();


            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe(int id, SubscriptionType subscriptionType)
        {
            var profile = await _context.UserProfile.FindAsync(id);
            if (profile == null) return NotFound();


            profile.SubscriptionType = subscriptionType;
            profile.SubscriptionStatus = SubscriptionStatus.PendingPayment;

            await _context.SaveChangesAsync();

            var amount = SubscriptionHelper.GetAmount(subscriptionType);
            var duration = SubscriptionHelper.GetDurationInMonths(subscriptionType);
            var transactionId = Guid.NewGuid().ToString();

            var payment = new SubscriptionPayment
            {
                UserProfileId = profile.UserProfileId,
                Amount = amount,
                PaidAt = DateTime.Now,
                TransactionId = transactionId,
                PaymentMethod = "SSLCOMMERZ"
            };

            _context.SubscriptionPayment.Add(payment);
            await _context.SaveChangesAsync();

            var storeId = "iubat68daa0ff0a96f";
            var storePassword = "iubat68daa0ff0a96f@ssl";
            var sslcUrl = "https://sandbox.sslcommerz.com/gwprocess/v4/api.php";

            var postData = new Dictionary<string, string>
{
    { "store_id", storeId },
    { "store_passwd", storePassword },
    { "total_amount", amount.ToString() },
    { "currency", "BDT" },
    { "tran_id", transactionId },
    { "success_url", $"{baseUrl}/AttendeeArea/Subscription/PaymentSuccess?tranId={transactionId}" },
    { "fail_url", $"{baseUrl}/AttendeeArea/Subscription/PaymentFail?tranId={transactionId}" },
    { "cancel_url", $"{baseUrl}/AttendeeArea/Subscription/PaymentCancel?tranId={transactionId}" },
    { "cus_name", profile.FullName ?? "Customer" },
    { "cus_email", profile.Email ?? "test@email.com" },
    { "cus_add1", profile.Address ?? "N/A" },
    { "cus_city", profile.City ?? "Dhaka" },
    { "cus_postcode", "1000" }, 
    { "cus_country", profile.Country ?? "Bangladesh" },
    { "cus_phone", profile.Phone ?? "01700000000" },
    { "shipping_method", "NO" },
    { "product_name", $"{profile.SubscriptionType} Subscription" },
    { "product_category", "Subscription" },
    { "product_profile", "general" }
};


            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(sslcUrl, new FormUrlEncodedContent(postData));
                var result = await response.Content.ReadAsStringAsync();

                var json = System.Text.Json.JsonDocument.Parse(result);
                var gatewayUrl = json.RootElement.GetProperty("GatewayPageURL").GetString();

                return Redirect(gatewayUrl);
            }
        }

        public async Task<IActionResult> PaymentSuccess(string tranId)
        {
            var payment = await _context.SubscriptionPayment
                .Include(p => p.UserProfile)
                .FirstOrDefaultAsync(p => p.TransactionId == tranId);

            if (payment == null) return NotFound();

            var profile = payment.UserProfile;

            profile.IsSubscribed = true;
            profile.SubscriptionStatus = SubscriptionStatus.Active;
            profile.SubscriptionStart = DateTime.Now;

            var duration = SubscriptionHelper.GetDurationInMonths(profile.SubscriptionType.Value);
            profile.SubscriptionExpiry = DateTime.Now.AddMonths(duration);
            profile.SubscriptionAmount = payment.Amount;

            _context.Update(profile);
            await _context.SaveChangesAsync();

            return View("PaymentSuccess", profile);
        }


        public IActionResult PaymentFail(string tranId)
        {
            ViewBag.Message = "Payment failed. Please try again.";
            return RedirectToAction("Profile");
        }

        public IActionResult PaymentCancel()
        {
            ViewBag.Message = "Payment cancelled.";
            return RedirectToAction("Profile");
        }


    }
}
