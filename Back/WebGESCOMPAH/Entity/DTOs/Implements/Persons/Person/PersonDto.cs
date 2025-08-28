namespace Entity.DTOs.Implements.Persons.Person
{
    public class PersonDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Document { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public int CityId { get; set; }
    }
}
