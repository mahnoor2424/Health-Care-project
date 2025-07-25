using System.ComponentModel.DataAnnotations;

namespace HealthCareConsultation.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public string ReceiverType { get; set; }  

        public int? SpecificDoctorId { get; set; }  

        [Required]
        public string NotificationType { get; set; }  

        [Required]
        [StringLength(500)]
        public string Message { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;
    }
}
