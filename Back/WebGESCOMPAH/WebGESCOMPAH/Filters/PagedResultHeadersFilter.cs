using Entity.DTOs.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebGESCOMPAH.Filters
{
    // Filtro global: si la acción devuelve PagedResult<T>, escribe headers de paginación.
    public sealed class PagedResultHeadersFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Result is ObjectResult objectResult && objectResult.Value is not null)
            {
                var valueType = objectResult.Value.GetType();
                if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(PagedResult<>))
                {
                    var total = valueType.GetProperty(nameof(PagedResult<object>.Total))?.GetValue(objectResult.Value);
                    var page = valueType.GetProperty(nameof(PagedResult<object>.Page))?.GetValue(objectResult.Value);
                    var size = valueType.GetProperty(nameof(PagedResult<object>.Size))?.GetValue(objectResult.Value);
                    var totalPages = valueType.GetProperty(nameof(PagedResult<object>.TotalPages))?.GetValue(objectResult.Value);

                    var headers = context.HttpContext.Response.Headers;

                    if (!headers.ContainsKey("X-Total-Count") && total is not null)
                        headers["X-Total-Count"] = total.ToString();
                    if (!headers.ContainsKey("X-Total-Pages") && totalPages is not null)
                        headers["X-Total-Pages"] = totalPages.ToString();
                    if (!headers.ContainsKey("X-Page") && page is not null)
                        headers["X-Page"] = page.ToString();
                    if (!headers.ContainsKey("X-Size") && size is not null)
                        headers["X-Size"] = size.ToString();
                }
            }

            await next();
        }
    }
}

