namespace Entity.DTOs.Implements.Business.Plaza
{
    public class PlazaSelectDto
    {
        public int Id { get; set; } // Unique identifier for the plaza
        public string Name { get; set; } = null!; // Name of the plaza
        public string Description { get; set; } = null!; // Description of the plaza
        public string Location { get; set; } = null!; // Location of the plaza
        public int Capacity { get; set; } // Maximum capacity of the plaza
        public bool Active { get; set; } // Indicates if the plaza is active
    }
}
