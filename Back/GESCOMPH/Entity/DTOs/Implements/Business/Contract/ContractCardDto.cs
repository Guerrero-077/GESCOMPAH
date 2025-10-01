    namespace Entity.DTOs.Implements.Business.Contract
    {
        public sealed record ContractCardDto(
            int Id,
            int PersonId,
            string FirstName,
            string LastName,
            string? PersonDocument,
            string? PersonPhone,
            string? PersonEmail,
            DateTime StartDate,
            DateTime EndDate,
            decimal TotalBase,
            decimal TotalUvt,
            bool Active)
        {
            public string PersonFullName => $"{FirstName} {LastName}".Trim();
        }

    }
