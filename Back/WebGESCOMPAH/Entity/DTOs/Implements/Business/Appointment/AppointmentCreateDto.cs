using Entity.Enum;

namespace Entity.DTOs.Implements.Business.Appointment
{
    public class AppointmentCreateDto : IAppointmentDto
    {
        // Data necesario para crear la persona
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? Document { get; set; }

        public string? Address { get; set; }

        public string? Phone { get; set; }

        public string Email { get; set; }

        public int CityId { get; set; }

        // Data necesaria para crea la cita

        public string Description { get; set; } = null!;

        public DateTime RequestDate { get; set; }

        public DateTime DateTimeAssigned { get; set; }

        //public int PersonId { get; set; }

        public int EstablishmentId { get; set; }

        public bool Active { get; set; }
    }
}
