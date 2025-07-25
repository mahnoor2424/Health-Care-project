using HealthCareConsultation.Data;
using HealthCareConsultation.Models;
using HealthCareConsultation.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HealthCareConsultation.Controllers
{
    public class AuthController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly HealthcareDbContext _context;

        public AuthController(IWebHostEnvironment environment, HealthcareDbContext context)
        {
            _environment = environment;
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(DoctorRegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var doctor = new DoctorProfile
            {
                FullName = model.FullName,
                Email = model.Email,
                Password = model.Password,
                PhoneNumber = model.PhoneNumber,
                Gender = model.Gender,
                Specialty = model.Specialty,
                Qualification = model.Qualification,
                Experience = model.Experience,
                AvailableDays = model.AvailableDays,
                AvailableTimeStart = model.AvailableTime,
                Role = model.Role,
                IsApproved = false,
                Bio = model.Bio
            };

            if (model.ProfileImage != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfileImage.FileName);
                string path = Path.Combine(_environment.WebRootPath, "images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(stream);
                }

                doctor.ProfileImage = fileName;
            }

            _context.DoctorProfiles.Add(doctor);
            _context.SaveChanges();

            TempData["Success"] = "✅ Your registration request has been sent to Admin.";
            return RedirectToAction("Register");
        }



     [HttpGet]
public IActionResult Login()
{
    
    return View(new Auth { Role = "Doctor" });
}
        [HttpPost]
        public async Task<IActionResult> Login(Auth model, string? returnUrl = null)
        {

            if (model.Role == "Doctor")
            {
                var doctor = await _context.DoctorProfiles
                    .FirstOrDefaultAsync(d => d.Email == model.Email && d.Password == model.Password);

                if (doctor != null)
                {
                    if (!doctor.IsApproved)
                        return Content("❗ Your account is not approved yet.");

                    HttpContext.Session.SetInt32("DoctorId", doctor.Id);
                    return RedirectToAction("Index", "DoctorDashboard");
                }
            }
            if (model.Role == "Patient")
            {

                System.Diagnostics.Debug.WriteLine("=== Login Attempt ===");
                System.Diagnostics.Debug.WriteLine("Role: " + model.Role);
                System.Diagnostics.Debug.WriteLine("Email: " + model.Email);
                System.Diagnostics.Debug.WriteLine("Password: " + model.Password);

                var patient = await _context.PatientProfiles
                    .FirstOrDefaultAsync(p => p.Email == model.Email);

                if (patient != null)
                {
                    HttpContext.Session.SetInt32("PatientId", patient.Id);
                    TempData["Success"] = "Successfully logged in as Patient.";

                    // ✅ Redirect back to where user came from
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction("Index", "Home"); // fallback
                }
            }

            return Content("❌ Login failed. User not found or password incorrect.");
        }







        [HttpPost]
        public IActionResult RegisterPatient(PatientProfile model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Register");

            _context.PatientProfiles.Add(model);
            _context.SaveChanges();

            TempData["Success"] = "✅ Patient Registered Successfully!";
            return RedirectToAction("Login");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Auth");
     
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}  