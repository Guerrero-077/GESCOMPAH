using Entity.DTOs.Implements.Utilities;

namespace Entity.DTOs.Implements.Business.Establishment
{
    public class EstablishmentSelectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public double AreaM2 { get; set; }
        public double RentValueBase { get; set; }
        public List<ImageDto> Images { get; set; } = new();
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
