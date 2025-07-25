namespace HealthCareConsultation.Models.ViewModels
{
    public class DoctorPrescriptionSummaryVM
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public int TotalPrescriptions { get; set; }
        public DateTime? LastPrescriptionOn { get; set; }
    }
}
