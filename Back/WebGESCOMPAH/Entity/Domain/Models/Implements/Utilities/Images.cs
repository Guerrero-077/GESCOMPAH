using Entity.Domain.Models.Implements.Business;
using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.Utilities
{
    public class Images : BaseModel
    {
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public int EstablishmentId { get; set; }
        public Establishment Establishment { get; set; } = null!;
    }
}
