using Entity.Domain.Models.ModelBase;
using Entity.Enum;

namespace Entity.Domain.Models.Implements.Business
{
    public class Appointment : BaseModel
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime RequestDate { get; set; } 
        public DateTime DateTimeAssigned { get; set; }
        public int EstablishmentId { get; set; }
        public int Status { get; set; }
        public  Establishment Establishment { get; set; }

    }
}
