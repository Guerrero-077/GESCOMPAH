using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Entity.DTOs.Implements.SecurityAuthentication.User;
using Entity.DTOs.Validations.Persons;
using FluentValidation;

namespace Entity.DTOs.Validations.SecurityAuthentication
{
    public class CreateUserValidator : BasePersonValidator<UserCreateDto>
    {
        private static readonly Regex DocumentRegex = new(@"^\d{7,10}$", RegexOptions.Compiled);

        public CreateUserValidator()
        {
            RuleFor(x => x.Document)
                .Cascade(CascadeMode.Stop)
                .Must(NotBlank)
                    .WithMessage("El documento es obligatorio.")
                .Must(value => DocumentRegex.IsMatch(value!.Trim()))
                    .WithMessage("El documento debe contener entre 7 y 10 digitos.");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .Must(NotBlank)
                    .WithMessage("El correo es obligatorio.")
                .Must(IsValidEmail)
                    .WithMessage("El correo no tiene un formato valido.");

            RuleFor(x => x.RoleIds)
                .Must(AllPositive)
                    .WithMessage("Todos los roles deben tener identificadores positivos.")
                .Must(AllDistinct)
                    .WithMessage("No se permiten roles duplicados.");
        }

        private static bool NotBlank(string? value) => !string.IsNullOrWhiteSpace(value);

        private static bool AllPositive(IReadOnlyCollection<int>? ids)
            => ids is null || ids.All(id => id > 0);

        private static bool AllDistinct(IReadOnlyCollection<int>? ids)
            => ids is null || ids.Distinct().Count() == ids.Count;

        private static bool IsValidEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                _ = new MailAddress(email.Trim());
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
