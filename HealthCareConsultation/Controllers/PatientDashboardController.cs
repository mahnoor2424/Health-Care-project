using Microsoft.AspNetCore.Mvc;

namespace HealthCareConsultation.Controllers
{
    public class PatientDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
