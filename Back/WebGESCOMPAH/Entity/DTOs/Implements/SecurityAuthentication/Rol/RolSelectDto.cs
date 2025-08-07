namespace Entity.DTOs.Implements.SecurityAuthentication.Rol
{
    public class RolSelectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool Active { get; set; }
    }
}
