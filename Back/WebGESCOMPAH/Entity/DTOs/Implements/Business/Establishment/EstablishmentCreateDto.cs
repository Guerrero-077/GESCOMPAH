using Microsoft.AspNetCore.Http;

namespace Entity.DTOs.Implements.Business.Establishment
{
    public class EstablishmentCreateDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public double AreaM2 { get; set; }
        public double RentValueBase { get; set; }
        // Archivos subidos desde el formulario
        public List<IFormFile>? Files { get; set; }
    }
}
