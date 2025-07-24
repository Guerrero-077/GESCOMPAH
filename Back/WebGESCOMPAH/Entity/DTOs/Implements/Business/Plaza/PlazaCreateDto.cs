namespace Entity.DTOs.Implements.Business.Plaza
{
    public class PlazaCreateDto 
    {
        public string Name { get; set; } = null!; // Name of the plaza
        public string Description { get; set; } = null!; // Description of the plaza
        public string Location { get; set; } = null!; // Location of the plaza
        public int Capacity { get; set; } // Maximum capacity of the plaza
    }
}
