using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.AdministrationSystem.SystemParameter
{
    public class SystemParameterUpdateDto : BaseDto
    {
        public string Key { get; set; }  = null!;
        public string Value { get; set; } = null!;
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}
