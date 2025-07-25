using HealthCareConsultation.Models;

namespace HealthCareConsultation.Models.ViewModels
{
    public class DoctorDashboardVM
    {
        public int TotalAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public int ApprovedAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int TodayAppointments { get; set; }

        public int TotalPrescriptions { get; set; }
        public int Last7DaysPrescriptions { get; set; }

        // Optional: show a small table of upcoming appts on dashboard
        public List<Appointment> UpcomingAppointments { get; set; } = new();
    }
}
