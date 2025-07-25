using System.ComponentModel.DataAnnotations;

namespace HealthCareConsultation.Models.ViewModels
{
    public class PatientRegisterViewModel
    {
        [Required] public string FullName { get; set; }
        [Required] public string Email { get; set; }
        [Required] public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
    }

}
