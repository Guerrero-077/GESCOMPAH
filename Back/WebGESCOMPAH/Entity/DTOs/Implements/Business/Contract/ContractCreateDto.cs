namespace Entity.DTOs.Implements.Business.Contract
{
    public class ContractCreateDto
    {
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required string Document { get; init; }
        public required string Phone { get; init; }
        public required string Address { get; init; }
        public required int CityId { get; init; }

        public string? Email { get; init; } // si lo envía, se genera usuario

        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }

        public List<int> EstablishmentIds { get; init; } = [];

        public decimal UvtQty { get; init; }

        public bool UseSystemParameters { get; init; } = true;

        /// <summary>Lista de cláusulas a asociar al contrato</summary>
        public List<int> ClauseIds { get; init; } = [];
    }
}
