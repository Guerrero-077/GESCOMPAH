using Entity.DTOs.Base;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Entity.DTOs.Implements.Utilities.Images
{

    public class ImageUpdateDto : BaseDto
    {

        [Required]
        public IFormFile File { get; set; }
    }


}
