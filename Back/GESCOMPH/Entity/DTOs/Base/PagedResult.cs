namespace Entity.DTOs.Base
{
    /// <summary>
    /// Resultado paginado estándar para UI o consumidores.
    /// </summary>
    public record PagedResult<T>(
        IEnumerable<T> Items,
        int Total,
        int Page,
        int Size
    )
    {
        public int TotalPages => (int)Math.Ceiling((double)Total / (Size <= 0 ? 1 : Size));
    }
}
