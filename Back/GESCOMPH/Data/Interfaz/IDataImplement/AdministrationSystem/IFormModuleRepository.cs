using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.AdministrationSystem;

namespace Data.Interfaz.IDataImplement.AdministrationSystem
{
    public interface IFormModuleRepository : IDataGeneric<FormModule>
    {
        Task<List<int>> GetUserIdsByFormIdAsync(int formId);

    }
}
