using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.AdministrationSystem.SystemParameter
{
    public class SystemParameterSelectDto : BaseDto
    {
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool Active { get; set; }
    }
}

