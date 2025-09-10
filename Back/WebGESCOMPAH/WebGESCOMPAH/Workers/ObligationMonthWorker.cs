//using Business.Interfaces.Implements.Business;

//namespace WebGESCOMPAH.Workers
//{
//    public sealed class ObligationMonthWorker : BackgroundService
//    {
//        private readonly ILogger<ObligationMonthWorker> _logger;
//        private readonly IServiceProvider _provider;
//        private readonly TimeSpan _period; // p. ej., se lee desde config

//        public ObligationMonthWorker(
//            IServiceProvider provider,
//            IConfiguration configuration,
//            ILogger<ObligationMonthWorker> logger)
//        {
//            _provider = provider;
//            _logger = logger;

//            // Ejemplo: periodo en días definido en configuración
//            int days = configuration.GetValue<int>("ObligationMonth:PeriodDays", 30);
//            _period = TimeSpan.FromDays(days);
//        }

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            using var timer = new PeriodicTimer(_period);

//            do
//            {
//                try
//                {
//                    using var scope = _provider.CreateScope();
//                    var service = scope.ServiceProvider.GetRequiredService<IObligationMonthService>();
//                    await service.GenerateForAllContractsAsync(stoppingToken);
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "Error al generar obligaciones mensuales");
//                }
//            }
//            while (await timer.WaitForNextTickAsync(stoppingToken));
//        }
//    }
