namespace Entity.DTOs.Implements.Business.EstablishmentDto
{
    /// <summary>
    /// DTO liviano para listados de establecimientos (tarjetas/grids).
    /// Incluye solo campos necesarios y una imagen principal opcional.
    /// </summary>
    public sealed class EstablishmentCardDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal AreaM2 { get; set; }
        public decimal RentValueBase { get; set; }
        public bool Active { get; set; }
        public string? PrimaryImagePath { get; set; }
    }
}
