namespace Entity.DTOs.Implements.Location.City
{
    public class CityCreateDto : ICityDto
    {
        public string Name { get; set; } = null!;
        public int DepartmentId { get; set; }
    }
}
