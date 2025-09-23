using System;

namespace Entity.DTOs.Implements.AdministrationSystem.SystemParameter
{
    public interface ISystemParameterDto
    {
        string Key { get; set; }
        string Value { get; set; }
        DateTime EffectiveFrom { get; set; }
        DateTime? EffectiveTo { get; set; }
    }
}
