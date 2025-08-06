using Entity.DTOs.Implements.Utilities.Images;
using Microsoft.AspNetCore.Http;

namespace Entity.DTOs.Implements.Business.EstablishmentDto
{
    public class EstablishmentUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public double AreaM2 { get; set; }
        public double RentValueBase { get; set; }
        public int PlazaId { get; set; }

        // Nuevas imágenes a subir
        //public ICollection<IFormFile>? Images { get; set; }
        public List<ImageSelectDto> Images { get; set; } = [];

    }

}