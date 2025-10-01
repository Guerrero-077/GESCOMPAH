using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.Business.Plaza
{
    public class UpdatePlazaActiveStatusDto: BaseDto
    {
        public bool Active { get; set; }
    }

}
