using Entity.DTOs.Base;
using Entity.DTOs.Implements.Utilities.Images;

namespace Entity.DTOs.Implements.Business.PremisesLeased
{
    public class PremisesLeasedSelectDto : BaseDto
    {
        public int EstablishmentId { get; set; }

        public string EstablishmentName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public double AreaM2 { get; set; }
        public decimal RentValueBase { get; set; }
        public string Address { get; set; } = null!;
        public string PlazaName { get; set; } = null!;

        public List<ImageSelectDto> Images { get; set; } = [];
    }
}
