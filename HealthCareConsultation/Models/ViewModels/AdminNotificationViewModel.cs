namespace HealthCareConsultation.Models.ViewModels
{
    public class AdminNotificationViewModel
    {
        public Notification NewNotification { get; set; }
        public List<Notification> Notifications { get; set; } = new List<Notification>();

        public List<DoctorProfile> Doctors { get; set; }
    }
}
