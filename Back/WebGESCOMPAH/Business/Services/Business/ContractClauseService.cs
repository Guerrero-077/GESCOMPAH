using Business.Interfaces.Implements.Business;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.ContractClause;
using MapsterMapper;
using System.Linq.Expressions;

namespace Business.Services.Business
{
    public class ContractClauseService : BusinessGeneric<ContractClauseSelectDto, ContractClauseDto, ContractClauseUpdateDto, ContractClause>, IContractClauseService
    {
        public ContractClauseService(IDataGeneric<ContractClause> data, IMapper mapper) : base(data, mapper)
        {
        }

        protected override Expression<Func<ContractClause, string>>[] SearchableFields() =>
            [
                cc => cc.Clause.Name!,
                cc => cc.Clause.Description!
            ];

        protected override string[] SortableFields() =>
        [
            nameof(ContractClause.ContractId),
        nameof(ContractClause.ClauseId),
        nameof(ContractClause.Id),
        nameof(ContractClause.CreatedAt),
        nameof(ContractClause.Active)
        ];

        protected override IDictionary<string, Func<string, Expression<Func<ContractClause, bool>>>> AllowedFilters() =>
            new Dictionary<string, Func<string, Expression<Func<ContractClause, bool>>>>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(ContractClause.ContractId)] = value => entity => entity.ContractId == int.Parse(value),
                [nameof(ContractClause.ClauseId)] = value => entity => entity.ClauseId == int.Parse(value),
                [nameof(ContractClause.Active)] = value => entity => entity.Active == bool.Parse(value)
            };
    }

}

