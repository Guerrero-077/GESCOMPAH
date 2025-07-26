using Microsoft.AspNetCore.Http;

namespace Entity.DTOs.Implements.Utilities.Images
{
    public class ImageUpdateDto
    {
        public int Id { get; set; } // ID de la imagen a actualizar
        public int? EstablishmentId { get; set; } // También opcional
        public IFormFile? File { get; set; } // Nueva imagen (opcional)
    }

}
