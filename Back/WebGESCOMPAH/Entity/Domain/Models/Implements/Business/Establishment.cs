using Entity.Domain.Models.Implements.Utilities;
using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.Business
{
    public class Establishment : BaseModelGeneric
    {
        public decimal AreaM2 { get; set; }
        public decimal RentValueBase { get; set; }
        public string Address { get; set; } = string.Empty;
        public decimal UvtQty { get; set; }

        public int PlazaId { get; set; }
        public Plaza? Plaza { get; set; }

        public List<Images> Images { get; set; } = [];
        public ICollection<Appointment> Appointments { get; set; } = [];
    }

}
