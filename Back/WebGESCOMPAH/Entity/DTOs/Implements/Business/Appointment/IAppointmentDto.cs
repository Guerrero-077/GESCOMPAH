namespace Entity.DTOs.Implements.Business.Appointment
{
    public interface IAppointmentDto
    {
        public string FullName { get; set; }
        public string Email { get; set; } 
        public string Phone { get; set; } 
        public int EstablishmentId { get; set; }
        public string Description { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime DateTimeAssigned { get; set; }
    }
}
