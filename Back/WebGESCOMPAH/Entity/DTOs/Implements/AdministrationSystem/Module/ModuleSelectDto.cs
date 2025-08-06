namespace Entity.DTOs.Implements.AdministrationSystem.Module
{
    public class ModuleSelectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Icon { get; set; } = null!;
        public bool Active { get; set; }
    }
}
