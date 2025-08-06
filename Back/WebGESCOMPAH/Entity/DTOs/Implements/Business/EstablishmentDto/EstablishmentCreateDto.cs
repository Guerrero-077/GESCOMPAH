using Microsoft.AspNetCore.Http;

namespace Entity.DTOs.Implements.Business.EstablishmentDto
{
    public class EstablishmentCreateDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public double AreaM2 { get; set; }
        public decimal RentValueBase { get; set; }
        public string Address { get; set; } = string.Empty;
        public int PlazaId { get; set; }

        /// <summary>
        /// Archivos de imagen asociados (máximo 5).
        /// </summary>
        public ICollection<IFormFile>? Files { get; set; }
    }

}
