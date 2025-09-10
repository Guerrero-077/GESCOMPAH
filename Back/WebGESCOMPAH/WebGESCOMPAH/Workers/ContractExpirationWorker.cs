using Business.Interfaces.Implements.Business;
using Microsoft.AspNetCore.SignalR;
using WebGESCOMPAH.RealTime;

namespace WebGESCOMPAH.Workers
{
    public sealed class ContractExpirationWorker : BackgroundService
    {
        private readonly ILogger<ContractExpirationWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<ContractsHub> _hub;
        private readonly TimeSpan _period;
        private readonly bool _enabled;

        public ContractExpirationWorker(
            ILogger<ContractExpirationWorker> logger,
            IServiceScopeFactory scopeFactory,
            IConfiguration cfg,
            IHubContext<ContractsHub> hub)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _hub = hub;

            _enabled = cfg.GetValue("Contracts:Expiration:Enabled", true);
            var minutes = cfg.GetValue("Contracts:Expiration:Minutes", 60);
            //_period = TimeSpan.FromMinutes(minutes); // ← periodo real en minutos
            _period = TimeSpan.FromSeconds(minutes); // ← periodo real en minutos
            // Para pruebas rápidas, puedes usar: _period = TimeSpan.FromSeconds(30);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_enabled)
            {
                _logger.LogInformation("ContractExpirationWorker deshabilitado por configuración.");
                return;
            }

            _logger.LogInformation("ContractExpirationWorker iniciado. Periodo: {Period}", _period);

            using var timer = new PeriodicTimer(_period);

            // Barrido inicial inmediato (no esperar el primer tick)
            await RunAndNotifyAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await timer.WaitForNextTickAsync(stoppingToken);
                await RunAndNotifyAsync(stoppingToken);
            }
        }

        private async Task RunAndNotifyAsync(CancellationToken ct)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var svc = scope.ServiceProvider.GetRequiredService<IContractService>();

                _logger.LogInformation("Barrido de expiración: INICIO");
                var result = await svc.RunExpirationSweepAsync(ct);
                _logger.LogInformation("Barrido de expiración: FIN OK");

                if (result.DeactivatedContractIds.Count > 0 || result.ReactivatedEstablishments > 0)
                {
                    await _hub.Clients.All.SendAsync("contracts:expired", new
                    {
                        deactivatedIds = result.DeactivatedContractIds,
                        counts = new
                        {
                            deactivatedContracts = result.DeactivatedContractIds.Count,
                            reactivatedEstablishments = result.ReactivatedEstablishments
                        },
                        at = DateTime.UtcNow
                    }, ct);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ContractExpirationWorker");
            }
        }

    }
}
