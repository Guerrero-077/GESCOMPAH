using Business.Interfaces.Implements.AdministrationSystem;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.DTOs.Implements.AdministrationSystem.Form;
using MapsterMapper;
using System.Linq.Expressions;

namespace Business.Services.AdministrationSystem
{
    public class FormService : BusinessGeneric<FormSelectDto, FormCreateDto, FormUpdateDto, Form>, IFormService
    {
        public FormService(IDataGeneric<Form> data, IMapper mapper) : base(data, mapper)
        {
        }
        // Aquí defines la llave “única” de negocio
        protected override IQueryable<Form>? ApplyUniquenessFilter(IQueryable<Form> query, Form candidate)
            => query.Where(f => f.Name == candidate.Name);

        protected override Expression<Func<Form, string>>[] SearchableFields() =>
        [
            f => f.Name!,
            f => f.Description!,
            f => f.Route!
        ];


        protected override IDictionary<string, Func<string, Expression<Func<Form, bool>>>> AllowedFilters() =>
            new Dictionary<string, Func<string, Expression<Func<Form, bool>>>>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(Form.Route)] = value => entity => entity.Route == value,
                [nameof(Form.Active)] = value => entity => entity.Active == bool.Parse(value)
            };
    }
}