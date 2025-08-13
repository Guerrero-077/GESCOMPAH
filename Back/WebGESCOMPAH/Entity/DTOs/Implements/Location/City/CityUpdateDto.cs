using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.Location.City
{
    public class CityUpdateDto : BaseDto, ICityDto
    {
        public string Name { get; set; } = null!;
        public int DepartmentId { get; set; }
    }
}
