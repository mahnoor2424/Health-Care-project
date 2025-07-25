using HealthCareConsultation.Data;
using HealthCareConsultation.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareConsultation.Controllers
{
    public class DoctorNotificationController : Controller
    {
        private readonly HealthcareDbContext _context;

        public DoctorNotificationController(HealthcareDbContext context)
        {
            _context = context;
        }

        public int? DoctorId { get; private set; }

        public IActionResult Index()
        {
            string patientId = HttpContext.Session.GetString("DoctorId");

            var notifications = _context.Notifications
                .Where(n =>
                    n.ReceiverType == "All Doctors" ||
                    (n.ReceiverType == "Specific Doctors" && n.SpecificDoctorId == DoctorId))
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            var vm = new DoctorNotificationViewModel
            {
                Notifications = notifications
            };

            return View(vm);
        }

    }
}
