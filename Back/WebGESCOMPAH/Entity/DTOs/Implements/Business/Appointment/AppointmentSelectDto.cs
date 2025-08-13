using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.Business.Appointment
{
    public class AppointmentSelectDto : BaseDto, IAppointmentDto
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool Active { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime DateTimeAssigned { get; set; }

        // Relación con Establecimiento
        public int EstablishmentId { get; set; }
        public string EstablishmentName { get; set; } = null!;
    }
}
