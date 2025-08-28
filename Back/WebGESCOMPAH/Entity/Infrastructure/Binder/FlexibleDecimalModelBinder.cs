// Infrastructure/Binder/FlexibleDecimalModelBinder.cs
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace Entity.Infrastructure.Binder
{
    public sealed class FlexibleDecimalModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext ctx)
        {
            var result = ctx.ValueProvider.GetValue(ctx.ModelName);
            if (result == ValueProviderResult.None) return Task.CompletedTask;

            var raw = result.FirstValue;
            if (string.IsNullOrWhiteSpace(raw))
            {
                if (ctx.ModelType == typeof(decimal?))
                    ctx.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            var normalized = NormalizeDecimal(raw);
            if (decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out var val))
            {
                ctx.Result = ModelBindingResult.Success(val);
            }
            else
            {
                ctx.ModelState.TryAddModelError(ctx.ModelName, "Valor decimal inválido.");
            }
            return Task.CompletedTask;
        }

        // Acepta "1234,56", "1.234,56", "1,234.56", "2.4", "2,4"
        private static string NormalizeDecimal(string s)
        {
            s = s.Trim();

            var hasDot = s.Contains('.');
            var hasComma = s.Contains(',');

            if (hasDot && hasComma)
            {
                // El ÚLTIMO separador es el decimal
                int lastDot = s.LastIndexOf('.');
                int lastComma = s.LastIndexOf(',');
                char dec = lastDot > lastComma ? '.' : ',';
                char thou = dec == '.' ? ',' : '.';
                s = s.Replace(thou.ToString(), ""); // quita miles
                s = s.Replace(dec, '.');            // usa punto como decimal
                return s;
            }

            if (hasComma)
            {
                // asume coma decimal; quita puntos por si fueron miles
                s = s.Replace(".", "");
                s = s.Replace(',', '.');
                return s;
            }

            if (hasDot)
            {
                // asume punto decimal; quita comas por si fueran miles
                s = s.Replace(",", "");
                return s; // ya tiene '.'
            }

            return s; // entero puro
        }
    }
}
