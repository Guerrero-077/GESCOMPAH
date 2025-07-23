namespace Entity.DTOs.Implements.Persons.Peron
{
    public class PersonUpdateDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Document { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public int CityId { get; set; }
    }
}
