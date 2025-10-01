using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.Business.Plaza
{
    public class PlazaUpdateDto : BaseDto, IPlazaDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Location { get; set; } = null!;
        //public int Capacity { get; set; }
    }
}