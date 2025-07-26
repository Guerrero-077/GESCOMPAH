using Microsoft.AspNetCore.Http;

namespace Entity.DTOs.Implements.Business.Establishment
{
    public class EstablishmentUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public double AreaM2 { get; set; }
        public double RentValueBase { get; set; }
    }

}
