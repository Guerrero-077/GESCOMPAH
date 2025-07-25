namespace Entity.DTOs.Implements.Business.Appointment
{
    public class AppointmentSelectDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime RequestDate { get; set; }
        public DateTime DareTimeAssigned { get; set; }
    }
}
