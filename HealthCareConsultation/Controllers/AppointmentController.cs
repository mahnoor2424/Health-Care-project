using HealthCareConsultation.Data;
using HealthCareConsultation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HealthCareConsultation.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly HealthcareDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private bool IsAdmin()
        {
            return HttpContext.Session.GetInt32("AdminId") != null
                || string.Equals(HttpContext.Session.GetString("Role"), "Admin", StringComparison.OrdinalIgnoreCase);
        }

        public AppointmentController(HealthcareDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            var patientId = _httpContextAccessor.HttpContext.Session.GetInt32("PatientId");
            if (patientId == null)
            {
                // Handle case if patient is not logged in or session expired
                return RedirectToAction("Login", "Account");
            }

            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Prescriptions)  // Important: load prescriptions with appointments
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDateTime)
                .ToListAsync();

            return View(appointments);
        }
        // GET: Appointment/Book
        public async Task<IActionResult> Book()
        {
            var doctors = await _context.DoctorProfiles.ToListAsync();
            ViewBag.Doctors = doctors;
            return View();
        }

        // POST: Appointment/Book
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(int doctorId, DateTime appointmentDateTime, string problemDescription)
        {
            var patientId = _httpContextAccessor.HttpContext.Session.GetInt32("PatientId");

            if (patientId == null)
            {
                return RedirectToAction("Login", "Auth");
            }


            // Manual validation
            if (doctorId <= 0)
            {
                ModelState.AddModelError("", "Please select a doctor");
            }

            if (appointmentDateTime <= DateTime.Now)
            {
                ModelState.AddModelError("", "Appointment date must be in the future");
            }

            if (string.IsNullOrWhiteSpace(problemDescription))
            {
                ModelState.AddModelError("", "Problem description is required");
            }

            if (ModelState.IsValid)
            {
                var appointment = new Appointment
                {
                    PatientId = patientId.Value,
                    DoctorId = doctorId,
                    AppointmentDateTime = appointmentDateTime,
                    ProblemDescription = problemDescription,
                    Status = "Pending"
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                return RedirectToAction("MyAppointments");
            }

            ViewBag.Doctors = await _context.DoctorProfiles.ToListAsync();
            return View();
        }

        // GET: Appointment/MyAppointments
        public async Task<IActionResult> MyAppointments()
        {
            var patientId = _httpContextAccessor.HttpContext.Session.GetInt32("PatientId");

            if (patientId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDateTime)
                .ToListAsync();

            var appointmentIds = appointments.Select(a => a.Id).ToList();
            var prescriptions = await _context.Prescriptions
                .Where(p => appointmentIds.Contains((int)p.AppointmentId))
                .ToListAsync();

            ViewBag.Prescriptions = prescriptions;

            return View(appointments);
        }

        public async Task<IActionResult> DoctorAppointments()
        {
            var doctorId = _httpContextAccessor.HttpContext.Session.GetInt32("DoctorId");
            if (doctorId == null) return RedirectToAction("Login", "Auth");

            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId)
                .OrderBy(a => a.AppointmentDateTime)
                .ToListAsync();


            var appointmentIds = appointments.Select(a => a.Id).ToList();

            // Load prescriptions
            var prescriptions = await _context.Prescriptions
                .Where(p => appointmentIds.Contains((int)p.AppointmentId))
                .ToListAsync();

            ViewBag.Prescriptions = prescriptions;

            return View(appointments);
        }

        public async Task<IActionResult> AllAppointments()
        {
           

            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .OrderByDescending(a => a.AppointmentDateTime)
                .ToListAsync();

            return View(appointments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            // Validate the requesting user is the appointment's doctor
            var doctorId = _httpContextAccessor.HttpContext.Session.GetInt32("DoctorId");
            if (doctorId == null || appointment.DoctorId != doctorId)
            {
                return Unauthorized();
            }

            appointment.Status = status;
            if (status == "Completed") appointment.CompletedAt = DateTime.Now;

            try
            {
                _context.Update(appointment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Appointment marked as {status}!";
            }
            catch
            {
                TempData["ErrorMessage"] = "Error updating appointment";
            }

            return RedirectToAction(nameof(DoctorAppointments));
        }

        // Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Appointment model)
        {
            // Remove navigation properties from validation
            ModelState.Remove("Doctor");
            ModelState.Remove("Patient");

            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid data" });

            var existingAppointment = await _context.Appointments.FindAsync(model.Id);
            if (existingAppointment == null)
                return Json(new { success = false, message = "Appointment not found" });

            // ---- Auth Checks ----
            var patientId = HttpContext.Session.GetInt32("PatientId");
            var doctorId = HttpContext.Session.GetInt32("DoctorId");
            var isAdmin = HttpContext.Session.GetInt32("AdminId") != null
                            || string.Equals(HttpContext.Session.GetString("Role"), "Admin", StringComparison.OrdinalIgnoreCase);

            if (!isAdmin &&
                (patientId == null || existingAppointment.PatientId != patientId) &&
                (doctorId == null || existingAppointment.DoctorId != doctorId))
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            // ---- Update common fields ----
            existingAppointment.AppointmentDateTime = model.AppointmentDateTime;
            existingAppointment.ProblemDescription = model.ProblemDescription;

            // ---- Only Doctor OR Admin can change Status ----
            if (doctorId != null || isAdmin)
            {
                existingAppointment.Status = model.Status;
                if (model.Status == "Completed")
                    existingAppointment.CompletedAt = DateTime.Now;
                else
                    existingAppointment.CompletedAt = null;
            }

            try
            {
                _context.Update(existingAppointment);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    id = existingAppointment.Id,
                    dateTime = existingAppointment.AppointmentDateTime.ToString("g"),
                    description = existingAppointment.ProblemDescription,
                    status = existingAppointment.Status,
                    statusClass = GetStatusClass(existingAppointment.Status)
                });
            }
            catch (Exception ex)
            {
                // log ex if needed
                return Json(new { success = false, message = "Server error while updating appointment." });
            }
        }

        private string GetStatusClass(string status)
        {
            switch (status)
            {
                case "Pending": return "bg-warning";
                case "Approved": return "bg-success";
                case "Completed": return "bg-primary";
                case "Cancelled": return "bg-danger";
                default: return "bg-secondary";
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            // Verify authorization
            var patientId = _httpContextAccessor.HttpContext.Session.GetInt32("PatientId");
            var doctorId = _httpContextAccessor.HttpContext.Session.GetInt32("DoctorId");
            var isAdmin = _httpContextAccessor.HttpContext.Session.GetInt32("AdminId") != null
                            || string.Equals(_httpContextAccessor.HttpContext.Session.GetString("Role"), "Admin", StringComparison.OrdinalIgnoreCase);

            if (!isAdmin &&
                (patientId == null || appointment.PatientId != patientId) &&
                (doctorId == null || appointment.DoctorId != doctorId))
            {
                return Unauthorized();
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Appointment deleted successfully!";

            // Redirect based on role
            if (isAdmin)
                return RedirectToAction("AllAppointments");
            else if (doctorId != null)
                return RedirectToAction("DoctorAppointments");
            else
                return RedirectToAction("MyAppointments");
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatusAjax(int id, string status)
        {
            var isAdmin = HttpContext.Session.GetInt32("AdminId") != null
                          || HttpContext.Session.GetString("Role") == "Admin";

            var doctorId = HttpContext.Session.GetInt32("DoctorId");

            var appt = _context.Appointments.Find(id);
            if (appt == null) return Json(new { success = false, message = "Appointment not found" });

            if (!isAdmin && (doctorId == null || appt.DoctorId != doctorId))
                return Json(new { success = false, message = "Unauthorized" });

            appt.Status = status;
            appt.CompletedAt = status == "Completed" ? DateTime.Now : null;

            _context.Update(appt);
            _context.SaveChanges();

            return Json(new
            {
                success = true,
                status = appt.Status,
                statusClass = GetStatusClass(appt.Status)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditAjax(Appointment model)
        {
            ModelState.Remove("Doctor");
            ModelState.Remove("Patient");

            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid data" });

            var appt = _context.Appointments.Find(model.Id);
            if (appt == null)
                return Json(new { success = false, message = "Appointment not found" });

            var isAdmin = HttpContext.Session.GetInt32("AdminId") != null
                          || HttpContext.Session.GetString("Role") == "Admin";
            var patientId = HttpContext.Session.GetInt32("PatientId");
            var doctorId = HttpContext.Session.GetInt32("DoctorId");

            if (!isAdmin &&
                (patientId == null || appt.PatientId != patientId) &&
                (doctorId == null || appt.DoctorId != doctorId))
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            appt.AppointmentDateTime = model.AppointmentDateTime;
            appt.ProblemDescription = model.ProblemDescription;

            if (doctorId != null || isAdmin)
            {
                appt.Status = model.Status;
                appt.CompletedAt = model.Status == "Completed" ? DateTime.Now : null;
            }

            _context.Update(appt);
            _context.SaveChanges();

            return Json(new
            {
                success = true,
                id = appt.Id,
                dateTime = appt.AppointmentDateTime.ToString("g"),
                description = appt.ProblemDescription,
                status = appt.Status,
                statusClass = GetStatusClass(appt.Status)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAjax(int id)
        {
            var appt = _context.Appointments.Find(id);
            if (appt == null) return Json(new { success = false, message = "Appointment not found" });

            var isAdmin = HttpContext.Session.GetInt32("AdminId") != null
                          || HttpContext.Session.GetString("Role") == "Admin";
            var patientId = HttpContext.Session.GetInt32("PatientId");
            var doctorId = HttpContext.Session.GetInt32("DoctorId");

            if (!isAdmin &&
                (patientId == null || appt.PatientId != patientId) &&
                (doctorId == null || appt.DoctorId != doctorId))
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            _context.Appointments.Remove(appt);
            _context.SaveChanges();

            return Json(new { success = true });
        }

    }
}