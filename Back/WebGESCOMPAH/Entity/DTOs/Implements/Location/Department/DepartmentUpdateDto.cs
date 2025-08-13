using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.Location.Department
{
    public class DepartmentUpdateDto : BaseDto
    {
        public string Name { get; set; } = null!;
    }
}
