using System;
using System.ComponentModel.DataAnnotations;

namespace HealthCareConsultation.Models
{
    public class DoctorProfile
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public string Gender { get; set; }

        public string? ProfileImage { get; set; } 

        [Required]
        public string Specialty { get; set; }

        [Display(Name = "Qualification")]
        public string Qualification { get; set; }

        [Range(0, 100)]
        public int Experience { get; set; }

        [Display(Name = "Available Days")]
        public string AvailableDays { get; set; }

        [Display(Name = "Available Start Time")]
        public TimeSpan? AvailableTimeStart { get; set; }

        [StringLength(1000)]
        public string Bio { get; set; }

        // ✅ Role is for future login handling (like 'Doctor', 'Admin')
        public string Role { get; set; } = "Doctor";

        // ✅ Approval is for self-registration case
        public bool IsApproved { get; set; } = true;  // Default TRUE for admin-created doctors
        public TimeSpan AvailableTimeEnd { get; internal set; }
        public string? Name { get; internal set; }
    }
}
