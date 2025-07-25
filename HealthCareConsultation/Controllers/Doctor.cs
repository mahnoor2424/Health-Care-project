namespace HealthCareConsultation.Controllers
{
    internal class Doctor
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Specialty { get; set; }
        public string Qualification { get; set; }
        public string Experience { get; set; }
        public string AvailableDays { get; set; }
        public string AvailableTime { get; set; }
        public string ProfileImage { get; set; }
        public string Role { get; set; }
        public bool IsApproved { get; set; }
    }
}