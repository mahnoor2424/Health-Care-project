using System.Collections.Generic;
using HealthCareConsultation.Models;

namespace HealthCareConsultation.Models.ViewModels
{
    public class DoctorProfileViewModel
    {
        public DoctorProfile NewDoctor { get; set; } = new();
        public List<DoctorProfile> DoctorsList { get; set; } = new();
    }
}
