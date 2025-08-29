using Microsoft.AspNetCore.Http;

namespace Entity.DTOs.Implements.Business.EstablishmentDto
{
    public class EstablishmentCreateDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal AreaM2 { get; set; }
        public decimal RentValueBase { get; set; }
        public string Address { get; set; } = string.Empty;
        public decimal UvtQty { get; set; }
        public int PlazaId { get; set; }
    }


}
