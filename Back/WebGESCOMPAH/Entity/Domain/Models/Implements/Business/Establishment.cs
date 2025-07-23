using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.Business
{
    public class Establishment : BaseModelGeneric
    {
        public Double AreaM2 { get; set; } // Area of the establishment in square meters
        public Double RentValueBase { get; set; } // Monthly rent value for the establishment


    }
}
