using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.Location
{
    public class Department : BaseModel
    {
        public string Name { get; set; } = null!;

        public List<City> Cities { get; set; } = new();
    }
}
