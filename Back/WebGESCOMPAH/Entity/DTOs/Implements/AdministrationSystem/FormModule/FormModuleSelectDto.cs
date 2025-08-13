using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.AdministrationSystem.FormModule
{
    public class FormModuleSelectDto : BaseDto
    {
        public string FormName { get; set; }
        public int FormId { get; set; }
        public string ModuleName { get; set; } 
        public string ModuleId { get; set; }
        public bool Active { get; set; }
    }
}
