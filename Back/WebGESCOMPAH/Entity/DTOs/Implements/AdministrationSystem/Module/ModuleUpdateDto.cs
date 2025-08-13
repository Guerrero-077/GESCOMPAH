using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.AdministrationSystem.Module
{
    public class ModuleUpdateDto : BaseDto, IModuleDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
    }
}
