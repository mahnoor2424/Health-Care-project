using HealthCareConsultation.Data;
using HealthCareConsultation.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HealthCareConsultation.Controllers
{
    public class ApprovalRequestController : Controller
    {
        private readonly HealthcareDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ApprovalRequestController(HealthcareDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            var pendingDoctors = _context.DoctorProfiles
                .Where(d => !d.IsApproved)
                .ToList();

            return View(pendingDoctors);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var doctor = await _context.DoctorProfiles.FindAsync(id);

            if (doctor == null)
                return NotFound();

            doctor.IsApproved = true;

            if (!string.IsNullOrEmpty(doctor.ProfileImage))
            {
                string uploadsPath = Path.Combine(_env.WebRootPath, "uploads", doctor.ProfileImage);
                string imagesPath = Path.Combine(_env.WebRootPath, "images", doctor.ProfileImage);

                if (System.IO.File.Exists(uploadsPath) && !System.IO.File.Exists(imagesPath))
                {
                    System.IO.File.Copy(uploadsPath, imagesPath);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
