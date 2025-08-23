using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.Persons.Peron
{
    public class PersonUpdateDto : BaseDto
    {
       public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Document { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!; 
        public int CityId { get; set; }
    }
}
