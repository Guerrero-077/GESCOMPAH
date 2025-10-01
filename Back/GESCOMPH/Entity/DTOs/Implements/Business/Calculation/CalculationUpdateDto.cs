using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.Business.Calculation
{
    public class CalculationUpdateDto : CalculationCreateDto
    {
        public int Id { get; set; }
        public bool Active { get; set; }
    }
}
