using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using HealthCareConsultation.Hubs;
using System.Security.Claims;

namespace HealthCareConsultation.Controllers
{
    public class ChatController : Controller
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public IActionResult Index(int appointmentId)
        {
            ViewBag.AppointmentId = appointmentId;
            ViewBag.CurrentUser = User?.Identity?.Name ?? "Anonymous";
            return View("~/Views/Chat/Index.cshtml");
        }

        public IActionResult Admin(int appointmentId)
        {
            ViewBag.AppointmentId = appointmentId;
            ViewBag.CurrentUser = User?.Identity?.Name ?? "Anonymous";
            return View("~/Views/Chat/Admin.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> SendSystemNotification(int appointmentId, string message)
        {
            await _hubContext.Clients.Group(appointmentId.ToString())
                .SendAsync("ReceiveSystemMessage", message);

            return Ok();
        }
    }
}