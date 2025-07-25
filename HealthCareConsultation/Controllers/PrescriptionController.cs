using HealthCareConsultation.Data;
using HealthCareConsultation.Models;
using HealthCareConsultation.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

public class PrescriptionController : Controller
{
    private readonly HealthcareDbContext _context;

    public PrescriptionController(HealthcareDbContext context)
    {
        _context = context;
    }
 
    // === GET: Prescription/Create?appointmentId=5 ===
    public IActionResult Create(int appointmentId)
    {
        var model = new PrescriptionModel
        {
            AppointmentId = appointmentId
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PrescriptionModel model)
    {
        // Remove navigation property validation
        ModelState.Remove("Appointment");

        if (ModelState.IsValid)
        {
            model.CreatedAt = DateTime.Now;
            _context.Prescriptions.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Prescription submitted successfully!";
            return RedirectToAction("DoctorAppointments", "Appointment");
        }

        // Debug errors if validation fails
        var errors = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        TempData["ErrorMessage"] = "Error in prescription form.";
        Console.WriteLine("Prescription Create Errors: " + string.Join(", ", errors));

        return RedirectToAction("DoctorAppointments", "Appointment");
    }
    public IActionResult ViewPrescription(int appointmentId)
    {
        var prescription = _context.Prescriptions
                                   .FirstOrDefault(p => p.AppointmentId == appointmentId);

        if (prescription == null)
        {
            // Handle not found
            return NotFound("Prescription not found.");
        }

        return View(prescription);
    }

    // ====================== PARTIAL VIEW FOR PRESCRIPTION (Single) ======================


    // ====================== AJAX LOAD ALL DOCTOR PRESCRIPTIONS ======================
    public IActionResult DoctorAllPrescriptions()
    {
        // Doctor ka email (session se ya User.Identity se)
        var email = HttpContext.Session.GetString("DoctorEmail");

        var doctor = _context.DoctorProfiles
            .FirstOrDefault(d => d.Email == email);

        if (doctor == null)
            return View(new List<PrescriptionModel>());

        var prescriptions = _context.Prescriptions
            .Include(p => p.Appointment)
                .ThenInclude(a => a.Patient)
            .Where(p => p.Appointment.DoctorId == doctor.Id)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

        ViewBag.DoctorName = doctor.FullName;
        return View(prescriptions);
    }
    public async Task<IActionResult> DoctorPrescriptions()
    {
        var doctorId = HttpContext.Session.GetInt32("DoctorId");
        if (doctorId == null)
            return RedirectToAction("Login", "Auth");

        var prescriptions = await _context.Prescriptions
            .Include(p => p.Appointment)
                .ThenInclude(a => a.Patient)
            .Where(p => p.Appointment.DoctorId == doctorId.Value)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return View(prescriptions);
    }
    public IActionResult AllPrescriptions()
    {
        var doctorPrescriptions = _context.DoctorProfiles
            .Select(d => new
            {
                DoctorName = d.FullName,
                TotalPrescriptions = _context.Prescriptions
                    .Include(p => p.Appointment)
                    .Count(p => p.Appointment.DoctorId == d.Id)
            })
            .ToList();

        ViewBag.DoctorPrescriptions = doctorPrescriptions;
        return View("~/Views/Admin/AllPrescriptions.cshtml"); // Explicit path
    }

}
