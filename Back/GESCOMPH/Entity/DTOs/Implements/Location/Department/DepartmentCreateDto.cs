using Entity.DTOs.Implements.Location.City;

namespace Entity.DTOs.Implements.Location.Department
{
    public class DepartmentCreateDto
    {
        public string Name { get; set; } = null!;

        public List<CityCreateDto> Cities { get; set; } = new();
    }
}
