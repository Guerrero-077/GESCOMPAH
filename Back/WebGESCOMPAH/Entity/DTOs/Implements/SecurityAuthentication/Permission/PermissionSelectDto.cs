namespace Entity.DTOs.Implements.SecurityAuthentication.Permission
{
    public class PermissionSelectDto
    {
        public int Id { get; set; } 
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Active { get; set; }

    }
}
