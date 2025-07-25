// HomeController.cs
using HealthCareConsultation.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class HomeController : Controller
{
    private readonly HealthcareDbContext _context;

    public HomeController(HealthcareDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var doctors = await _context.DoctorProfiles
            .Where(d => d.IsApproved == true)
            .ToListAsync();

        return View(doctors); // Pass to view
    }
}
