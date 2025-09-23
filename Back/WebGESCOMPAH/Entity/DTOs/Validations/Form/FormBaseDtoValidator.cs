using System;
using System.Text.RegularExpressions;
using Entity.DTOs.Implements.AdministrationSystem.Form;
using FluentValidation;

namespace Entity.DTOs.Validations.Form
{
    public abstract class FormBaseDtoValidator<T> : AbstractValidator<T> where T : IFormDto
    {
        private const int NameMaxLength = 100;
        private const int DescriptionMaxLength = 500;
        private const int RouteMaxLength = 200;
        private static readonly Regex RouteRegex = new Regex("^[A-Za-z0-9/_\\-]+$", RegexOptions.Compiled);

        protected FormBaseDtoValidator()
        {
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .Must(value => !string.IsNullOrWhiteSpace(value)).WithMessage("El nombre es obligatorio.")
                .MaximumLength(NameMaxLength).WithMessage($"El nombre no puede superar {NameMaxLength} caracteres.");

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .Must(value => !string.IsNullOrWhiteSpace(value)).WithMessage("La descripción es obligatoria.")
                .MaximumLength(DescriptionMaxLength).WithMessage($"La descripción no puede superar {DescriptionMaxLength} caracteres.");

            RuleFor(x => x.Route)
                .Cascade(CascadeMode.Stop)
                .Custom((value, context) =>
                {
                    var trimmed = value?.Trim();
                    if (string.IsNullOrEmpty(trimmed))
                    {
                        context.AddFailure("Route", "La ruta es obligatoria.");
                        return;
                    }

                    if (trimmed.Length > RouteMaxLength)
                    {
                        context.AddFailure("Route", $"La ruta no puede superar {RouteMaxLength} caracteres.");
                        return;
                    }

                    if (!RouteRegex.IsMatch(trimmed))
                    {
                        context.AddFailure("Route", "La ruta solo puede contener letras, números, guiones, guiones bajos y '/'.");
                    }
                });
        }
    }
}
