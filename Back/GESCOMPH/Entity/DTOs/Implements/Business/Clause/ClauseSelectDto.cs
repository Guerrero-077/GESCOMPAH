using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.Business.Clause
{
    public class ClauseSelectDto : BaseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
