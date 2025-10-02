using Event_Management_And_Ticket_Booking_System.Data;
using Event_Management_And_Ticket_Booking_System.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Event_Management_And_Ticket_Booking_System.Areas.AttendeeArea.Controllers
{
    [Area("AttendeeArea")]
    public class MyEventController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _web;
        public MyEventController(ApplicationDbContext context, IWebHostEnvironment web)
        {
            _context = context;
            _web = web;
        }
        public IActionResult Index()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var enrolledEvents = _context.Booking
                    .Where(b => b.UserId == currentUserId)
                    .Include(b => b.Event)
                    .Select(b => b.Event)
                    .Distinct()
                    .ToList();

            return View(enrolledEvents);
        }

        public IActionResult MyGallery(int eventId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var photos = _context.EventPhoto
                .Where(p => p.EventId == eventId && p.UploadedBy == currentUserId)
                .ToList();

            ViewBag.EventId = eventId;
            return View(photos);
        }
        [HttpGet]
        public async Task<IActionResult> DownloadAll(int eventId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var photos = _context.EventPhoto
                .Where(p => p.EventId == eventId && p.UploadedBy == currentUserId)
                .ToList();

            if (!photos.Any())
                return RedirectToAction("MyGallery", new { eventId });

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new System.IO.Compression.ZipArchive(memoryStream, System.IO.Compression.ZipArchiveMode.Create, true))
                {
                    foreach (var photo in photos)
                    {
                        var filePath = Path.Combine(_web.WebRootPath, photo.FilePath.TrimStart('/').Replace("/", "\\"));
                        if (System.IO.File.Exists(filePath))
                        {
                            var entry = archive.CreateEntry(photo.FileName);
                            using (var entryStream = entry.Open())
                            using (var fileStream = System.IO.File.OpenRead(filePath))
                            {
                                await fileStream.CopyToAsync(entryStream);
                            }
                        }
                    }
                }

                return File(memoryStream.ToArray(), "application/zip", $"Event_{eventId}_Photos.zip");
            }
        }


        [HttpPost]
        public async Task<IActionResult> UploadPhotos(int eventId, List<IFormFile> Photos)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (Photos == null || Photos.Count == 0)
                return RedirectToAction("MyGallery", new { eventId });

            string folderPath = Path.Combine(_web.WebRootPath, "EventPhotos", eventId.ToString(), currentUserId);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            foreach (var photo in Photos)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(photo.FileName);
                string filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                var eventPhoto = new EventPhoto
                {
                    EventId = eventId,
                    UploadedBy = currentUserId,
                    FileName = fileName,
                    FilePath = $"/EventPhotos/{eventId}/{currentUserId}/{fileName}",
                    UploadDateTime = DateTime.UtcNow
                };

                _context.EventPhoto.Add(eventPhoto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("MyGallery", new { eventId });
        }
    }

}
