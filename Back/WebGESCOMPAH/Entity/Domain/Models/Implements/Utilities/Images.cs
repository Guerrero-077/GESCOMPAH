using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.Utilities
{
    public class Images: BaseModel
    {
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;

    }
}
