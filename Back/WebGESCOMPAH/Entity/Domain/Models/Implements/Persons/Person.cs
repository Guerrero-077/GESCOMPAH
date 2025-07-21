using Entity.Domain.Models.Implements.Location;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.Persons
{
    public class Person: BaseModel
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? Document { get; set; }

        public string? Address { get; set; }

        public string? Phone { get; set; }

        public int CityId { get; set; }

        public City City { get; set; } = null!;

        public User? User { get; set; }
    }

}
