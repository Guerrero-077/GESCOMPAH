namespace Entity.DTOs.Implements.SecurityAuthentication.Me
{
    public class FormDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Route { get; set; }
        //public List<MenuModuleDto> Modules { get; set; }
        public IEnumerable<string> Permissions { get; set; } = [];
    }
}
