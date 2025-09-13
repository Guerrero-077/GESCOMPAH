namespace Entity.DTOs.Implements.Business.Contract
{
    public sealed record ExpirationSweepResult(
        IReadOnlyList<int> DeactivatedContractIds,
        int ReactivatedEstablishments);
}

