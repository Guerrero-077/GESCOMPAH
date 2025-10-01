using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.SecurityAuthentication
{
    public class RolUser : BaseModel
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int RolId { get; set; }
        public Rol Rol { get; set; } = null!;
    }
}
