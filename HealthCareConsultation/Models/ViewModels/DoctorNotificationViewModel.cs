using System.Collections.Generic;
using HealthCareConsultation.Models;


namespace HealthCareConsultation.Models.ViewModels
{
    public class DoctorNotificationViewModel
    {
        public List<Notification> Notifications { get; set; } = new();
    }
}
