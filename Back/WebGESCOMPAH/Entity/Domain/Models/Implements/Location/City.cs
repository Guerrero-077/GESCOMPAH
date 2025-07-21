using Entity.Domain.Models.Implements.Persons;
using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.Location
{
    public class City : BaseModel
    {
        public string Name { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public List<Person>? People { get; set; } = new();
    }
}
