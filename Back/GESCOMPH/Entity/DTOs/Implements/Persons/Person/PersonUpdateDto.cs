using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.Persons.Person
{
    public class PersonUpdateDto : BaseDto
    {
       public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!; 
        public int CityId { get; set; }
    }
}
