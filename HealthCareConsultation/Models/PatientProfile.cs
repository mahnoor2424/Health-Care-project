using System.ComponentModel.DataAnnotations;

namespace HealthCareConsultation.Models
{
    public class PatientProfile
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public string Role { get; set; } = "Patient";
        public string? Name { get; internal set; }
    }

}
