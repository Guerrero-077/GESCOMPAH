using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Persons;
using Microsoft.EntityFrameworkCore;
namespace Data.Interfaz.IDataImplement.Persons
{
    public interface IPersonRepository : IDataGeneric<Person>
    {
        Task<Person?> GetByIdWithCityAsync(int id);
        Task<bool> ExistsByDocumentAsync(string document);

        Task<Person?> GetByDocumentAsync(string document);

    }
}
