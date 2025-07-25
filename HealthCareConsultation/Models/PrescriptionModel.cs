using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareConsultation.Models
{
    public class PrescriptionModel
    {
        [Key]
        public int Id { get; set; }

     
        public int AppointmentId { get; set; }   // Yahan add karein

        [ForeignKey("AppointmentId")]
        public Appointment Appointment { get; set; }

        [Required]
        public string Symptoms { get; set; }

        [Required]
        public string Diagnosis { get; set; }

        [Required]
        public string Medicines { get; set; } // e.g., JSON or plain string

        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
