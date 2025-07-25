using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareConsultation.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }  

        [ForeignKey("PatientId")]
        public PatientProfile Patient { get; set; } 

        [Required]
        public int DoctorId { get; set; }  

        [ForeignKey("DoctorId")]
        public DoctorProfile Doctor { get; set; }  

        [Required]
        public DateTime AppointmentDateTime { get; set; }

        public string Status { get; set; } = "Pending";

        [Required]
        public string ProblemDescription { get; set; }

        public DateTime? CompletedAt { get; set; }
        public virtual ICollection<PrescriptionModel> Prescriptions { get; set; } = new List<PrescriptionModel>();
    }
}