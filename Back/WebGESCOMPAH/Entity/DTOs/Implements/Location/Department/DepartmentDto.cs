using Entity.DTOs.Implements.Location.City;

namespace Entity.DTOs.Implements.Location.Department
{
    public class DepartmentDto
    {
        public string Name { get; set; }

        public List<CityDto> Cities { get; set; } = new();
    }
}
