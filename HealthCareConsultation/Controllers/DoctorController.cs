using HealthCareConsultation.Data;
using HealthCareConsultation.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HealthCareConsultation.Controllers
{
    public class DoctorController : Controller
    {
        private readonly HealthcareDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DoctorController(HealthcareDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Profile()
        {
            int? doctorId = HttpContext.Session.GetInt32("DoctorId");
            if (doctorId == null)
                return RedirectToAction("Login", "Auth");

            var doctor = await _context.DoctorProfiles.FindAsync(doctorId);
            if (doctor == null)
                return NotFound();

            return View(doctor); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(DoctorProfile model, IFormFile? profileImage)
        {
            int? doctorId = HttpContext.Session.GetInt32("DoctorId");
            if (doctorId == null)
                return RedirectToAction("Login", "Auth");

            var doctor = await _context.DoctorProfiles.AsNoTracking().FirstOrDefaultAsync(d => d.Id == model.Id);
            if (doctor == null)
                return NotFound();

            // ✅ Preserve original image
            string existingImage = doctor.ProfileImage;

            // ✅ Update fields
            doctor.FullName = model.FullName;
            doctor.Email = model.Email;
            doctor.PhoneNumber = model.PhoneNumber;
            doctor.Gender = model.Gender;
            doctor.Specialty = model.Specialty;
            doctor.Qualification = model.Qualification;
            doctor.Experience = model.Experience;
            doctor.AvailableDays = model.AvailableDays;
            doctor.AvailableTimeStart = model.AvailableTimeStart;
            doctor.AvailableTimeEnd = model.AvailableTimeEnd;
            doctor.Bio =  model.Bio;

            // ✅ Image logic
            if (profileImage != null && profileImage.Length > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(profileImage.FileName);
                var extension = Path.GetExtension(profileImage.FileName);
                var uniqueName = $"{fileName}_{Guid.NewGuid()}{extension}";
                var folderPath = Path.Combine(_env.WebRootPath, "images");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, uniqueName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profileImage.CopyToAsync(stream);
                }

                doctor.ProfileImage = "/images/" + uniqueName;
                HttpContext.Session.SetString("DoctorImage", doctor.ProfileImage);
            }
            else
            {
                doctor.ProfileImage = existingImage;
            }

            HttpContext.Session.SetString("DoctorName", doctor.FullName);

            _context.DoctorProfiles.Update(doctor);
            await _context.SaveChangesAsync();

            ViewBag.Message = "✅ Profile updated successfully!";
            return View(doctor);
        }

    }
}
