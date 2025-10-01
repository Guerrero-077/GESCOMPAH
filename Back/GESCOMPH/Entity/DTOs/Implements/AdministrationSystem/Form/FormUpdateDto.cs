using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.AdministrationSystem.Form
{
    public class FormUpdateDto : BaseDto, IFormDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Route { get; set; }
    }
}