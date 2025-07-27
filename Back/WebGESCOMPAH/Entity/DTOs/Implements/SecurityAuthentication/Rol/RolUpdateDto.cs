namespace Entity.DTOs.Implements.SecurityAuthentication.Rol
{
    public class RolUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}
