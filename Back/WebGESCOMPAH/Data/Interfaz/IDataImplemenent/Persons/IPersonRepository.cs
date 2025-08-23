using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Persons;
namespace Data.Interfaz.IDataImplemenent.Persons
{
    public interface IPersonRepository : IDataGeneric<Person>
    {
        Task<Person?> GetByIdWithCityAsync(int id);
        Task<bool> ExistsByDocumentAsync(string document);
    }
}
