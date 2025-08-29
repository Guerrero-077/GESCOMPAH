using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.Business.EstablishmentDto
{
    public class EstablishmentUpdateDto : BaseDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Address { get; set; } = null!;
        public decimal AreaM2 { get; set; }
        public decimal RentValueBase { get; set; }
        public decimal UvtQty { get; set; }
        public int PlazaId { get; set; }
    }
}