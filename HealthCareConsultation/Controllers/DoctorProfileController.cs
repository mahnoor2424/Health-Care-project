using HealthCareConsultation.Data;
using HealthCareConsultation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using HealthCareConsultation.Models.ViewModels;
using static System.Net.Mime.MediaTypeNames;
using System;

namespace HealthCareConsultation.Controllers
{
    public class DoctorProfileController : Controller
    {
        private readonly HealthcareDbContext _context;

        public DoctorProfileController(HealthcareDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new DoctorProfileViewModel
            {
                NewDoctor = new DoctorProfile(),
                DoctorsList = await _context.DoctorProfiles
                                            .Where(d => d.IsApproved == true)
                                            .ToListAsync()
            };

            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoctorProfileViewModel vm, IFormFile ProfileImage)
        {
            if (ModelState.IsValid)
            {
                var doctor = vm.NewDoctor;

                doctor.Role = "Doctor";
                doctor.IsApproved = true;

                if (ProfileImage != null && ProfileImage.Length > 0)
                {
                    var fileName = Path.GetFileNameWithoutExtension(ProfileImage.FileName);
                    var extension = Path.GetExtension(ProfileImage.FileName);
                    var uniqueName = $"{fileName}_{Guid.NewGuid()}{extension}";
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    var filePath = Path.Combine(folderPath, uniqueName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProfileImage.CopyToAsync(stream);
                    }

                    doctor.ProfileImage = "/images/" + uniqueName;
                }

                if (string.IsNullOrWhiteSpace(doctor.Password))
                {
                    doctor.Password = "Doctor@123"; 
                }

                _context.DoctorProfiles.Add(doctor);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Doctor added successfully!";
                TempData["NewDoctorInfo"] = $"Login with Email: {doctor.Email} and Password: {doctor.Password}";

                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = string.Join(" | ", ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .SelectMany(x => x.Value.Errors.Select(e => $"{x.Key}: {e.ErrorMessage}")));

            vm.DoctorsList = await _context.DoctorProfiles.ToListAsync();
            return View("Index", vm);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var doctor = await _context.DoctorProfiles.FirstOrDefaultAsync(d => d.Id == id);
            if (doctor == null) return NotFound();

            return View(doctor);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DoctorProfile doctorProfile, IFormFile? NewProfileImage)

        {
            if (id != doctorProfile.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingDoctor = await _context.DoctorProfiles.FirstOrDefaultAsync(d => d.Id == id);
                    if (existingDoctor == null)
                        return NotFound();

                    // ✅ Preserve old image if new one not uploaded
                    if (NewProfileImage != null && NewProfileImage.Length > 0)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(NewProfileImage.FileName);
                        var extension = Path.GetExtension(NewProfileImage.FileName);
                        var uniqueName = $"{fileName}_{Guid.NewGuid()}{extension}";
                        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        var filePath = Path.Combine(folderPath, uniqueName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await NewProfileImage.CopyToAsync(stream);
                        }

                        doctorProfile.ProfileImage = "/images/" + uniqueName;
                    }
                    else
                    {
                        doctorProfile.ProfileImage = existingDoctor.ProfileImage; // 👈 keep old image
                    }

                    // ✅ Preserve password if not updated
                    if (string.IsNullOrEmpty(doctorProfile.Password))
                    {
                        doctorProfile.Password = existingDoctor.Password;
                    }

                    _context.Entry(existingDoctor).State = EntityState.Detached; // important
                    _context.Update(doctorProfile);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "✅ Doctor updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    TempData["Error"] = "❌ Error while updating.";
                    return RedirectToAction(nameof(Index));
                }
            }

            TempData["Error"] = string.Join(" | ", ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .SelectMany(x => x.Value.Errors.Select(e => $"{x.Key}: {e.ErrorMessage}")));

            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _context.DoctorProfiles.FindAsync(id);
            if (doctor != null)
            {
                _context.DoctorProfiles.Remove(doctor);
                await _context.SaveChangesAsync();
                TempData["Success"] = "✅ Doctor deleted successfully!";
            }
            else
            {
                TempData["Error"] = "❌ Doctor not found!";
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
