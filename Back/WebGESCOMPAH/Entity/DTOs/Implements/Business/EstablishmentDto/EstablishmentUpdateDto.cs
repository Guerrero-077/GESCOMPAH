    using Entity.DTOs.Implements.Utilities.Images;
    using Microsoft.AspNetCore.Http;

namespace Entity.DTOs.Implements.Business.EstablishmentDto
{
    public class EstablishmentUpdateDto
    {
        public int Id { get; set; }          // <-- requerido
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string address { get; set; } = null!;
        public double AreaM2 { get; set; }
        public double RentValueBase { get; set; }
        public int PlazaId { get; set; }

        /// <summary>Archivos de imagen a subir (máx. 5)</summary>
        public List<IFormFile>? Images { get; set; }

        /// <summary>PublicIds de las imágenes que se eliminarán</summary>
        public List<string>? ImagesToDelete { get; set; }
    }

}