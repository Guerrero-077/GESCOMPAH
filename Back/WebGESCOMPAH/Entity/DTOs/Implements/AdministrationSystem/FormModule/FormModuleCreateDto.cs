namespace Entity.DTOs.Implements.AdministrationSystem.FormModule
{
    public class FormModuleCreateDto: IFormModuleDto
    {
        public int FormId { get; set; }
        public int ModuleId { get; set; }
    }
}
