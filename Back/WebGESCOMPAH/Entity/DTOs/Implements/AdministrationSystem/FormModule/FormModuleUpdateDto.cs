using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.AdministrationSystem.FormModule
{
    public class FormModuleUpdateDto : BaseDto, IFormModuleDto
    {
        public int FormId { get; set; }

        public int ModuleId { get; set; }
    }
}
