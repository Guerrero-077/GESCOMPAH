namespace Entity.DTOs.Implements.Persons.Peron
{
    public class PersonSelectDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Document { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string CityName { get; set; }
    }
}
