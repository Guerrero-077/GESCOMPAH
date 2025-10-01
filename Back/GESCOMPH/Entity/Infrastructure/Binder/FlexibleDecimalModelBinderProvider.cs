using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Entity.Infrastructure.Binder
{
    public sealed class FlexibleDecimalModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            var t = context.Metadata.ModelType;
            if (t == typeof(decimal) || t == typeof(decimal?))
                return new FlexibleDecimalModelBinder();
            return null;
        }
    }
}
