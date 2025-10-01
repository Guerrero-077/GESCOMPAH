using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.Business.Appointment
{
    public class AppointmentUpdateDto : BaseDto, IAppointmentDto
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public int EstablishmentId { get; set; }
        public string Description { get; set; } = null!;
        public DateTime RequestDate { get; set; }
        public DateTime DateTimeAssigned { get; set; }
    }
}