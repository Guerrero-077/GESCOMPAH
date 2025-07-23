using Entity.Domain.Models.Implements.Persons;
using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.Business
{
    public class Contract : BaseModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Destination { get; set; } = null!;
        public string PDFPath { get; set; } = null!;
    }
}
