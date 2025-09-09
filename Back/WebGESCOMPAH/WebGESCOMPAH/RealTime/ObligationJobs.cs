using Business.Interfaces.Implements.Business;
using Hangfire;
using TimeZoneConverter;

namespace WebGESCOMPAH.RealTime
{
    public sealed class ObligationJobs
    {
        private readonly IObligationMonthService _svc;
        private readonly ILogger<ObligationJobs> _log;
        private readonly IConfiguration _cfg;

        public ObligationJobs(IObligationMonthService svc, ILogger<ObligationJobs> log, IConfiguration cfg)
        {
            _svc = svc;
            _log = log;
            _cfg = cfg;
        }

        // Evita solapamiento si una ejecución mensual tarda más de lo previsto
        [DisableConcurrentExecution(timeoutInSeconds: 60 * 60)]
        public async Task GenerateForCurrentMonthAsync(CancellationToken ct)
        {
            var tzId = _cfg["Hangfire:TimeZoneIana"] ?? "America/Bogota";
            var tz = TZConvert.GetTimeZoneInfo(tzId);
            var nowLocal = TimeZoneInfo.ConvertTime(DateTime.UtcNow, tz);

            var year = nowLocal.Year;
            var month = nowLocal.Month;

            _log.LogInformation("Generando obligaciones para {Year}-{Month}", year, month);
            await _svc.GenerateMonthlyAsync(year, month);
            _log.LogInformation("OK obligaciones {Year}-{Month}", year, month);
        }

        // ✅ Nuevo: para encolar ejecución de un periodo específico
        [DisableConcurrentExecution(timeoutInSeconds: 60 * 60)]
        [Queue("maintenance")]
        public async Task GenerateForPeriodAsync(int year, int month, CancellationToken ct)
        {
            if (month is < 1 or > 12)
            {
                _log.LogWarning("Mes inválido {Month} para año {Year}", month, year);
                return;
            }

            _log.LogInformation("Generando obligaciones (ad-hoc) para {Year}-{Month}", year, month);
            await _svc.GenerateMonthlyAsync(year, month);
            _log.LogInformation("OK obligaciones (ad-hoc) {Year}-{Month}", year, month);
        }
    }
}
