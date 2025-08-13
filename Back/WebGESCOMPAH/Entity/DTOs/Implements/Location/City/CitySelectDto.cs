using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.Location.City
{
    public class CitySelectDto : BaseDto, ICityDto
    {
        public string Name { get; set; }
        public string DepartmentName { get; set; }
        public bool Active { get; set; }
    }
}
