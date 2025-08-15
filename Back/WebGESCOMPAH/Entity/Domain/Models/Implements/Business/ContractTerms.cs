using Entity.Domain.Models.ModelBase;
using System.Diagnostics.Contracts;

namespace Entity.Domain.Models.Implements.Business
{
    public class ContractTerms : BaseModel
    {
        public Double ValueWeights { get; set; }
        public Double ValueUVT { get; set; }
        public enum Frequency
        {
            Monthly,
            Quarterly,
            SemiAnnually,
            Annually
        }
    }
}
