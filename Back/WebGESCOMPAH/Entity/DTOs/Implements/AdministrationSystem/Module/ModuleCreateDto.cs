namespace Entity.DTOs.Implements.AdministrationSystem.Module
{
    public class ModuleCreateDto : IModuleDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
    }
}
