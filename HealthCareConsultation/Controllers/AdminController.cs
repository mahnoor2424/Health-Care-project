using System.Linq;
using System.Threading.Tasks;
using HealthCareConsultation.Data;
using HealthCareConsultation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Microsoft.EntityFrameworkCore;

namespace HealthCareConsultation.Controllers
{
    public class AdminController : Controller
    {
        private readonly HealthcareDbContext _context;

        public AdminController(HealthcareDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> AllPrescriptions(int? doctorId)
        {
            IQueryable<PrescriptionModel> query = _context.Prescriptions
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Patient)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Doctor);

            // Agar doctor select kiya hai, filter karo
            if (doctorId.HasValue && doctorId.Value > 0)
            {
                query = query.Where(p => p.Appointment.DoctorId == doctorId.Value);
            }

            var prescriptions = await query
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();


            // 👇 Yahan SelectList banayen (selected value = doctorId)
            ViewBag.Doctors = new SelectList(
                await _context.DoctorProfiles
                    .AsNoTracking()
                    .Select(d => new { d.Id, d.FullName })
                    .ToListAsync(),
                "Id",
                "FullName",
                doctorId
            );

            return View("~/Views/Admin/AllPrescriptions.cshtml", prescriptions);
        }


        // DETAILS (no AJAX → normal full view)
        [HttpGet]
        public async Task<IActionResult> ViewPrescription(int id)
        {
            var prescription = await _context.Prescriptions
                .AsNoTracking()
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Patient)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Doctor)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prescription == null)
                return NotFound();

            return View("~/Views/Admin/ViewPrescription.cshtml", prescription);
        }
    }
}
