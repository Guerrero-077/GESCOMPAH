using Entity.Domain.Models.Implements.Persons;
using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.Business
{
    public class Appointment : BaseModel
    {
        public string Description { get; set; }
        public DateTime RequestDate { get; set; } 
        public DateTime? DateTimeAssigned { get; set; }
        public string? Observation { get; set; }

        // Ralacion con Persona
        public int PersonId { get; set; }
        public Person Person { get; set; }

        // Relacion con Establecimiento
        public int EstablishmentId { get; set; }
        public  Establishment Establishment { get; set; }

  
        public bool Active { get; set; }

        //public int Status { get; set; }

    }
}
