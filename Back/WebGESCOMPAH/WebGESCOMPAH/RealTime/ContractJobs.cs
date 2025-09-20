using Business.Interfaces.Implements.Business;

public sealed class ContractJobs
{
    private readonly ILogger<ContractJobs> _logger;
    private readonly IContractService _contractService;

    public ContractJobs(
        ILogger<ContractJobs> logger,
        IContractService contractService)
    {
        _logger = logger;
        _contractService = contractService;
    }

    public async Task RunExpirationSweepAsync(CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Barrido de contratos vencidos: INICIO");

            var result = await _contractService.RunExpirationSweepAsync(ct);

            _logger.LogInformation(
                "Barrido de contratos vencidos: FIN OK. Deactivated={Deactivated}, Reactivated={Reactivated}",
                result.DeactivatedContractIds.Count,
                result.ReactivatedEstablishments
            );  
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ContractJobs.RunExpirationSweepAsync");
        }
    }
}
