using Entity.Domain.Models.Implements.Location;

namespace Entity.DTOs.Implements.Location
{
    public class DepartmentDto
    {
        public string Name { get; set; }

        public List<City> Cities { get; set; } = new();
    }
}
