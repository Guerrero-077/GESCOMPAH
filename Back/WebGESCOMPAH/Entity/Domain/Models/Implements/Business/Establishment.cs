using Entity.Domain.Models.Implements.Utilities;
using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.Business
{
    public class Establishment : BaseModelGeneric
    {
        public Double AreaM2 { get; set; } // Area of the establishment in square meters
        public Double RentValueBase { get; set; } // Monthly rent value for the establishment
        public string Address { get; set; } = string.Empty;
        public decimal UvtQty { get; set; }

        public int PlazaId { get; set; } // Foreign key to the Plaza entity
        public Plaza? Plaza { get; set; } // Navigation property to the Plaza entity

        public List<Images> Images { get; set; } = [];
        public ICollection<Appointment> Appointments { get; set; } = [];
    }
}
