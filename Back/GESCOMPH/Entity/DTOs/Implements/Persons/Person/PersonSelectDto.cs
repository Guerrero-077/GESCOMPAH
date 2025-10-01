using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.Persons.Person
{
    public class PersonSelectDto : BaseDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Document { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string CityName { get; set; } = null!;
        public int CityId { get; set; }

        public string? Email { get; set; }
    }
}
