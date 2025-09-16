namespace Entity.DTOs.Implements.Business.EstablishmentDto
{
    /// <summary>
    /// Proyección liviana para cálculos: Id, RentValueBase, UvtQty.
    /// </summary>
    public sealed class EstablishmentBasicsDto
    {
        public int Id { get; set; }
        public decimal RentValueBase { get; set; }
        public decimal UvtQty { get; set; }
    }
}

