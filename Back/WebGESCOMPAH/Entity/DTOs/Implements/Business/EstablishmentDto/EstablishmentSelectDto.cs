using Entity.DTOs.Implements.Utilities.Images;

namespace Entity.DTOs.Implements.Business.EstablishmentDto
{ 
    public class EstablishmentSelectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public double AreaM2 { get; set; }
        public decimal RentValueBase { get; set; }
        public string Address { get; set; } = String.Empty;
        public int PlazaId { get; set; }
        public string PlazaName { get; set; } = null!;
        public bool Active { get; set; }

        /// <summary>
        /// Imágenes asociadas al establecimiento
        /// </summary>
        public List<ImageSelectDto> Images { get; set; } = [];
    }

}