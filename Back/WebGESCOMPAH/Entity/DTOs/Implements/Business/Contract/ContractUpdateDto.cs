namespace Entity.DTOs.Implements.Business.Contract
{
    public class ContractUpdateDto
    {
        public int Id { get; set; }
        public DateOnly StartDate { get; init; }
        public DateOnly EndDate { get; init; }
        public bool Active { get; init; }

        // Permite reconfigurar locales y recalcular totales
        public List<int> EstablishmentIds { get; init; } = [];

        // Reasignación de cláusulas
        public List<int> ClauseIds { get; init; } = [];

        // Recalcular obligaciones con parámetros del sistema si aplica
        public bool UseSystemParameters { get; init; } = true;
    }
}