using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.Utilities.Images
{
    public class ImageSelectDto : BaseDto
    {
        public ImageSelectDto(int id, string fileName, string filePath, string publicId, int establishmentId)
        {
            Id = id;
            FileName = fileName;
            FilePath = filePath;
            PublicId = publicId;
            EstablishmentId = establishmentId;
        }

        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public string PublicId { get; set; } = null!;
       public int  EstablishmentId { get; set; }
    }

}
