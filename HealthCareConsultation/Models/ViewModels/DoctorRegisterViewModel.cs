using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace HealthCareConsultation.Models.ViewModels
{
    public class DoctorRegisterViewModel
    {
        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        public string Gender { get; set; }

        public string Specialty { get; set; }

        public string Qualification { get; set; }

        public int Experience { get; set; }

        public string AvailableDays { get; set; }

        public TimeSpan? AvailableTime { get; set; }
      
        public string Bio { get; set; }
        public IFormFile ProfileImage { get; set; }
    }
}
