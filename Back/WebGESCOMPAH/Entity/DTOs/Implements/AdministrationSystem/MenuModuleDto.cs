namespace Entity.DTOs.Implements.AdministrationSystem
{
    public class MenuModuleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<FormDto> Forms { get; set; } = [];
    }
}
