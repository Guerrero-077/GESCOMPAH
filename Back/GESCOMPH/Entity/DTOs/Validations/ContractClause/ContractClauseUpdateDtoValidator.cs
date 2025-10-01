using Entity.DTOs.Implements.Business.ContractClause;
using FluentValidation;

namespace Entity.DTOs.Validations.ContractClause
{
    public class ContractClauseUpdateDtoValidator : AbstractValidator<ContractClauseUpdateDto>
    {
        public ContractClauseUpdateDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                    .WithMessage("El identificador es obligatorio.");

            RuleFor(x => x.ContractId)
                .GreaterThan(0)
                    .WithMessage("Debe seleccionar un contrato valido.");

            RuleFor(x => x.ClauseId)
                .GreaterThan(0)
                    .WithMessage("Debe seleccionar una clausula valida.");
        }
    }
}
