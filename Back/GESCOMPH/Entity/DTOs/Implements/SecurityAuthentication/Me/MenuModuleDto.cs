namespace Entity.DTOs.Implements.SecurityAuthentication.Me
{
    public class MenuModuleDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public IEnumerable<FormDto> Forms { get; set; } = [];
    }
}
