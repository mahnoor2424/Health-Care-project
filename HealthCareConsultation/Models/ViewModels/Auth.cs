using System.ComponentModel.DataAnnotations;

namespace HealthCareConsultation.Models.ViewModels
{
    public class Auth
    {
        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // "Doctor" or "Patient"
        public bool IsApproved { get; set; } = false;

    }
}
