using Microsoft.AspNetCore.Http;

namespace Entity.DTOs.Implements.Utilities.Images
{
    public class ImageCreateDto
    {
        public int EstablishmentId { get; set; }
        //public ICollection<IFormFile> Files { get; set; } = new List<IFormFile>();
        public List<IFormFile>? Files { get; set; }
        //public string FileName { get; set; } = null!;
        //public string FilePath { get; set; } = null!;
    }

}
