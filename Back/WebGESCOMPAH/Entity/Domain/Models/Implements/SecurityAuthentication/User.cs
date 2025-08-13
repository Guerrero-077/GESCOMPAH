using Entity.Domain.Models.Implements.Persons;
using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.SecurityAuthentication
{
    public class User: BaseModel
    {
        public string Email { get; set; } = null!;
        public string Password{ get; set; } = null!;

        public int PersonId { get; set; }
        public Person Person { get; set; } = null!;
        public ICollection<RolUser> RolUsers { get; set; } = new List<RolUser>(); // Relacion de muchos a muchos 
    }

}
