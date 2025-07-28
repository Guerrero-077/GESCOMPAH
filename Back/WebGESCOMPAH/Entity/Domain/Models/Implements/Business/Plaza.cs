using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.Business
{
    public class Plaza : BaseModelGeneric
    {
        public string Location { get; set; } = null!; // Location of the plaza
        public int Capacity { get; set; } // Maximum capacity of the plaza
        public List<Establishment> Establishments { get; set; } = []; // List of establishments in the plaza
    }
}
