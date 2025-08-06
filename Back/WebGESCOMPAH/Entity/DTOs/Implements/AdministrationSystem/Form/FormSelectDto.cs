namespace Entity.DTOs.Implements.AdministrationSystem.Form
{
    public class FormSelectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Route { get; set; }
        public bool Active { get; set; }
    }
}
