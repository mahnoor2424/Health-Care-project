using HealthCareConsultation.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HealthCareConsultation.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly HealthcareDbContext _context;

        public AdminDashboardController(HealthcareDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.TotalDoctors = _context.DoctorProfiles.Count();
            ViewBag.TotalPatients = _context.PatientProfiles.Count();
            ViewBag.TotalAppointments = _context.Appointments.Count();
            ViewBag.TotalPrescriptions = _context.Prescriptions.Count();

            return View();
        }

     

    }
}
