using Business.Interfaces.Implements.Business;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.Clause;
using MapsterMapper;
using System.Linq.Expressions;

namespace Business.Services.Business
{
    public class ClauseService : BusinessGeneric<ClauseSelectDto, ClauseDto, ClauseUpdateDto, Clause>, IClauseService
    {
        public ClauseService(IDataGeneric<Clause> data, IMapper mapper) : base(data, mapper)
        {
        }

        // Aquí defines la llave “única” de negocio
        protected override IQueryable<Clause>? ApplyUniquenessFilter(IQueryable<Clause> query, Clause candidate)
            => query.Where(c => c.Description == candidate.Description);

        protected override Expression<Func<Clause, string>>[] SearchableFields() =>
            [
                c => c.Name!,
                c => c.Description!
            ];

        protected override IDictionary<string, Func<string, Expression<Func<Clause, bool>>>> AllowedFilters() =>
               new Dictionary<string, Func<string, Expression<Func<Clause, bool>>>>(StringComparer.OrdinalIgnoreCase)
               {
                   [nameof(Clause.Description)] = value => entity => entity.Description == value,
                   [nameof(Clause.Active)] = value => entity => entity.Active == bool.Parse(value)
               };

        protected override string[] SortableFields() =>
            [
            nameof(Clause.Description),
            nameof(Clause.Id),
            nameof(Clause.CreatedAt),
            nameof(Clause.Active)
            ];
    }
}