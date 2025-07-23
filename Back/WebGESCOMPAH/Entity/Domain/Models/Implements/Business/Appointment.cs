using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.Business
{
    public class Appointment : BaseModel
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime RequestDate { get; set; } 
        public DateTime DareTimeAssigned { get; set; }

    }
}
