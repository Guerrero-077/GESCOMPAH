namespace Entity.DTOs.Implements.AdministrationSystem.Form
{
    public interface IFormDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Route { get; set; }
    }
}
