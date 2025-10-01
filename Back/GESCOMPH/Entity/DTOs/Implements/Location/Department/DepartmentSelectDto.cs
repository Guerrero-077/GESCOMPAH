using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.Location.Department
{
    public class DepartmentSelectDto : BaseDto
    {
        public string Name { get; set; }= null!;
        public bool Active { get; set; }
    }
}
