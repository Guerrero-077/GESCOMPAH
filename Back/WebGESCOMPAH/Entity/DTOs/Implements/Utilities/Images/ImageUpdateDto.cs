using Microsoft.AspNetCore.Http;

namespace Entity.DTOs.Implements.Utilities.Images
{

    public class ImageUpdateDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public string PublicId { get; set; } = null!;
        public int EstablishmentId { get; set; }
    }


}
