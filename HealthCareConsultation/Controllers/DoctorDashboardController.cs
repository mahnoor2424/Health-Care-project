using HealthCareConsultation.Data;
using HealthCareConsultation.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HealthCareConsultation.Controllers
{
    public class DoctorDashboardController : Controller
    {
        private readonly HealthcareDbContext _context;
        private readonly IHttpContextAccessor _http;

        public DoctorDashboardController(HealthcareDbContext context, IHttpContextAccessor http)
        {
            _context = context;
            _http = http;
        }

        // GET: /DoctorDashboard
        public async Task<IActionResult> Index()
        {
            var doctorId = _http.HttpContext.Session.GetInt32("DoctorId");
            if (doctorId == null)
                return RedirectToAction("Login", "Auth");

            var now = DateTime.Now;
            var today = now.Date;
            var last7Days = today.AddDays(-6);

            var doctorAppointments = _context.Appointments
                                             .Where(a => a.DoctorId == doctorId.Value);

            var vm = new DoctorDashboardVM
            {
                TotalAppointments = await doctorAppointments.CountAsync(),
                PendingAppointments = await doctorAppointments.CountAsync(a => a.Status == "Pending"),
                ApprovedAppointments = await doctorAppointments.CountAsync(a => a.Status == "Approved"),
                CompletedAppointments = await doctorAppointments.CountAsync(a => a.Status == "Completed"),
                CancelledAppointments = await doctorAppointments.CountAsync(a => a.Status == "Cancelled"),
                TodayAppointments = await doctorAppointments.CountAsync(a => a.AppointmentDateTime.Date == today),

                // Jo error aa rahi thi, usko avoid karne ke liye yeh queries explicit join/where ke sath likh di:
                TotalPrescriptions = await _context.Prescriptions
                                                   .Where(p => p.Appointment.DoctorId == doctorId.Value)
                                                   .CountAsync(),

                Last7DaysPrescriptions = await _context.Prescriptions
                                                       .Where(p => p.Appointment.DoctorId == doctorId.Value &&
                                                                   p.CreatedAt.Date >= last7Days &&
                                                                   p.CreatedAt.Date <= today)
                                                       .CountAsync(),

                UpcomingAppointments = await doctorAppointments
                                            .Include(a => a.Patient)
                                            .Where(a => a.AppointmentDateTime > now && a.Status != "Cancelled")
                                            .OrderBy(a => a.AppointmentDateTime)
                                            .Take(5)
                                            .ToListAsync()
            };

            return View(vm);
        }
    }
}
