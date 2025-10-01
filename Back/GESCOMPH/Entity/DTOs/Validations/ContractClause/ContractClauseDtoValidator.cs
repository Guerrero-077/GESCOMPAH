using Entity.DTOs.Implements.Business.ContractClause;
using FluentValidation;

namespace Entity.DTOs.Validations.ContractClause
{
    public class ContractClauseDtoValidator : AbstractValidator<ContractClauseDto>
    {
        public ContractClauseDtoValidator()
        {
            RuleFor(x => x.ContractId)
                .GreaterThan(0)
                    .WithMessage("Debe seleccionar un contrato valido.");

            RuleFor(x => x.ClauseId)
                .GreaterThan(0)
                    .WithMessage("Debe seleccionar una clausula valida.");
        }
    }
}
