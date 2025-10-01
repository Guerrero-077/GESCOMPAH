using System;

namespace Entity.DTOs.Implements.Business.Calculation
{
    public class CalculationCreateDto
    {
        public string Type { get; set; } = null!;
        public string Key { get; set; } = null!;
        public string Formula { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}
