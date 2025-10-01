namespace Entity.DTOs.Implements.Business.Appointment
{
    public interface IAppointmentDto
    {

        public int EstablishmentId { get; set; }
        public string Description { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime DateTimeAssigned { get; set; }
    }
}
