using System.Linq;
using Entity.DTOs.Implements.SecurityAuthentication.RolFormPemission;
using FluentValidation;

namespace Entity.DTOs.Validations.SecurityAuthentication.RolFormPermission
{
    public class RolFormPermissionUpdateDtoValidator : AbstractValidator<RolFormPermissionUpdateDto>
    {
        public RolFormPermissionUpdateDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                    .WithMessage("El identificador es obligatorio.");

            RuleFor(x => x.RolId)
                .GreaterThan(0)
                    .WithMessage("Debe seleccionar un rol valido.");

            RuleFor(x => x.FormId)
                .GreaterThan(0)
                    .WithMessage("Debe seleccionar un formulario valido.");

            RuleFor(x => x.PermissionIds)
                .Cascade(CascadeMode.Stop)
                .Must(list => list is { Count: > 0 })
                    .WithMessage("Debe especificar al menos un permiso.")
                .Must(AllPositive)
                    .WithMessage("Todos los permisos deben tener identificadores positivos.")
                .Must(AllDistinct)
                    .WithMessage("No se permiten permisos duplicados.");
        }

        private static bool AllPositive(IReadOnlyCollection<int>? ids)
            => ids is null || ids.All(id => id > 0);

        private static bool AllDistinct(IReadOnlyCollection<int>? ids)
            => ids is null || ids.Distinct().Count() == ids.Count;
    }
}
