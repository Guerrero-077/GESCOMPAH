namespace Entity.DTOs.Implements.Location.City
{
    public class CityUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int DepartmentId { get; set; }
    }
}
