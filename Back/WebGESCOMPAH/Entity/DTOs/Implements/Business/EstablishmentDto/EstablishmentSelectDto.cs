using Entity.DTOs.Base;
using Entity.DTOs.Implements.Utilities.Images;

namespace Entity.DTOs.Implements.Business.EstablishmentDto
{
    public class EstablishmentSelectDto : BaseDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal AreaM2 { get; set; }
        public decimal RentValueBase { get; set; }
        public string Address { get; set; } = string.Empty;
        public int PlazaId { get; set; }
        public string PlazaName { get; set; } = null!;
        public bool Active { get; set; }

        public decimal UvtQty { get; set; }

        public List<ImageSelectDto> Images { get; set; } = [];
    }
}