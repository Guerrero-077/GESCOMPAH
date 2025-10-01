namespace Entity.DTOs.Implements.Business.Contract
{
    public sealed record ContractCardDto(
        int Id,
        int PersonId,
        string PersonFullName,
        string? PersonDocument,
        string? PersonPhone,
        string? PersonEmail,
        DateTime StartDate,
        DateTime EndDate,
        decimal TotalBase,
        decimal TotalUvt,
        bool Active);
}
