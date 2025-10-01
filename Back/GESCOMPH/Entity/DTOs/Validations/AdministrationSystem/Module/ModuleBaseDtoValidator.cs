using System;
using System.Text.RegularExpressions;
using Entity.DTOs.Implements.AdministrationSystem.Module;
using FluentValidation;

namespace Entity.DTOs.Validations.AdministrationSystem.Module
{
    public abstract class ModuleBaseDtoValidator<T> : AbstractValidator<T> where T : IModuleDto
    {
        private const int NameMaxLength = 100;
        private const int DescriptionMaxLength = 500;
        private const int IconMaxLength = 100;
        private static readonly Regex IconRegex = new Regex("^[A-Za-z0-9_\\-]+$", RegexOptions.Compiled);

        protected ModuleBaseDtoValidator()
        {
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .Must(value => !string.IsNullOrWhiteSpace(value)).WithMessage("El nombre es obligatorio.")
                .MaximumLength(NameMaxLength).WithMessage($"El nombre no puede superar {NameMaxLength} caracteres.");

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .Must(value => !string.IsNullOrWhiteSpace(value)).WithMessage("La descripción es obligatoria.")
                .MaximumLength(DescriptionMaxLength).WithMessage($"La descripción no puede superar {DescriptionMaxLength} caracteres.");

            RuleFor(x => x.Icon)
                .Cascade(CascadeMode.Stop)
                .Must(value => !string.IsNullOrWhiteSpace(value)).WithMessage("El icono es obligatorio.")
                .MaximumLength(IconMaxLength).WithMessage($"El icono no puede superar {IconMaxLength} caracteres.")
                .Matches(IconRegex).WithMessage("El icono solo puede contener letras, números, guiones y guiones bajos.");
        }
    }
}
