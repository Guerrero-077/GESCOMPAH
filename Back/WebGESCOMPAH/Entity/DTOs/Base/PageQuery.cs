namespace Entity.DTOs.Base
{
    /// <summary>
    /// Parámetros de consulta para endpoints de listado.
    /// </summary>
    public record PageQuery(
        int Page = 1,                   // 1-based
        int Size = 20,                  // tamaño de página
        string? Search = null,          // texto libre sobre campos "buscables"
        string? Sort = null,            // nombre exacto de propiedad (ej: "Name")
        bool Desc = true,               // orden descendente por defecto
        IDictionary<string, string>? Filters = null // k=v exacto (ej: ["Status"]="Active")
    );
}
