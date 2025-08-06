namespace Entity.DTOs.Implements.SecurityAuthentication.Me
{
    public class UserMeDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;


        //public PersonDto Person { get; set; }
        //public List<RolUserDto> Roles { get; set; }
        ////public List<FormDto> Forms { get; set; }
        //public IEnumerable<MenuModuleDto> Menu { get; set; } = [];

        public IEnumerable<string> Roles { get; set; } = [];
        public IEnumerable<string> Permissions { get; set; } = [];

        public IEnumerable<MenuModuleDto> Menu { get; set; } = [];
    }



}
