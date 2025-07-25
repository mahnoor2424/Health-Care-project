using Microsoft.AspNetCore.Mvc;
using HealthCareConsultation.Models;
using HealthCareConsultation.Data;
using HealthCareConsultation.Models.ViewModels;

namespace HealthCareConsultation.Controllers
{
    public class NotificationController : Controller
    {
        private readonly HealthcareDbContext _context;

        public NotificationController(HealthcareDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var vm = new AdminNotificationViewModel
            {
                NewNotification = new Notification(),
                Doctors = _context.DoctorProfiles.ToList(),
                Notifications = _context.Notifications
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(50)
                    .ToList()
            };

            return View(vm);
        }



        [HttpPost]
        public IActionResult Send(Notification notification)
        {
            if (notification.ReceiverType != "Specific Doctor")
            {
                notification.SpecificDoctorId = null;
            }

            notification.CreatedAt = DateTime.Now;
            _context.Notifications.Add(notification);
            _context.SaveChanges();

            TempData["Success"] = "✅ Notification Sent!";
            return RedirectToAction("Index");
        }
    }
}
