using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.AdministrationSystem
{
    public class FormModule : BaseModel
    {
        public int FormId { get; set; }
        public Form Form { get; set; } = null!;

        public int ModuleId { get; set; }
        public Module Module { get; set; } = null!;
    }

}
