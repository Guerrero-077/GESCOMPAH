namespace Entity.DTOs.Implements.Business.Contract
{
    public class ContractCreateDto
    {
        // Persona (alta o reuso por documento)
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required string Document { get; init; }
        public required string Phone { get; init; }
        public required string Address { get; init; }
        public required int CityId { get; init; }

        public string? Email { get; init; }

        // Contrato
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }

        // Selección de locales (los totales salen de aquí)
        public List<int> EstablishmentIds { get; init; } = [];

        // Parámetros de cálculo
        public bool UseSystemParameters { get; init; } = true;

        /// <summary>Cláusulas a asociar</summary>
        public List<int> ClauseIds { get; init; } = [];
    }
}