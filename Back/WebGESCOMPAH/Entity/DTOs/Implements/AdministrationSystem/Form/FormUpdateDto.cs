namespace Entity.DTOs.Implements.AdministrationSystem.Form
{
    public class FormUpdateDto
    {
        public int Id { get; set;  }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Route { get; set; }
    }
}
