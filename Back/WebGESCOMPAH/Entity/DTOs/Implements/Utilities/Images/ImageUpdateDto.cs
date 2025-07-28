using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Entity.DTOs.Implements.Utilities.Images
{

    public class ImageUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public IFormFile File { get; set; }
    }


}
