using Business.Interfaces.Implements.Business;
using Entity.DTOs.Implements.Business.ObligationMonth;
using Hangfire;
using Hangfire.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TimeZoneConverter;
using WebGESCOMPAH.Controllers.Base;
using WebGESCOMPAH.RealTime;

namespace WebGESCOMPAH.Controllers.Module.Business
{
    [ApiController]
    [Route("api/obligation-months")]
    [Produces("application/json")]
    // [Authorize(Roles = "Administrador")]
    public sealed class ObligationMonthsController
           : BaseController<ObligationMonthSelectDto, ObligationMonthDto, ObligationMonthUpdateDto>
    {
        private readonly IObligationMonthService _svc;
        private readonly IBackgroundJobClient _jobs;
        private readonly IConfiguration _cfg;
        private readonly ILogger<ObligationMonthsController> _logger;

        public ObligationMonthsController(
            IObligationMonthService service,
            IBackgroundJobClient jobs,
            IConfiguration cfg,
            ILogger<ObligationMonthsController> logger
        ) : base(service, logger)
        {
            _svc = service;
            _jobs = jobs;
            _cfg = cfg;
            _logger = logger;
        }

        /// <summary>
        /// Ejecuta SINCRÓNICAMENTE el cálculo de obligaciones para el período indicado (o el actual).
        /// </summary>
        [HttpPost("generate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Generate([FromQuery] int? year, [FromQuery] int? month, CancellationToken ct)
        {
            var (y, m) = ResolvePeriod(year, month);
            if (m is < 1 or > 12) return BadRequest("El parámetro 'month' debe estar entre 1 y 12.");

            _logger.LogInformation("Generación SINCRONA de obligaciones para {Year}-{Month}", y, m);
            await _svc.GenerateMonthlyAsync(y, m);
            return Ok(new { message = "Obligaciones generadas/actualizadas", year = y, month = m });
        }

        /// <summary>
        /// Encola un job en Hangfire para calcular obligaciones del período indicado (o el actual).
        /// </summary>
        [HttpPost("enqueue")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Enqueue([FromQuery] int? year, [FromQuery] int? month)
        {
            var (y, m) = ResolvePeriod(year, month);
            if (m is < 1 or > 12) return BadRequest("El parámetro 'month' debe estar entre 1 y 12.");

            // Encola en la cola "maintenance" y pasa un IJobCancellationToken válido
            var jobId = _jobs.Enqueue<ObligationJobs>(
                j => j.GenerateForPeriodAsync(y, m, JobCancellationToken.Null));

            _logger.LogInformation("Job encolado para obligaciones {Year}-{Month}. JobId={JobId}", y, m, jobId);
            return Accepted(new { message = "Job encolado", jobId, year = y, month = m });
        }

        /// <summary>
        /// Dispara el recurring job 'obligations-monthly' tal cual está configurado.
        /// </summary>
        [HttpPost("trigger-recurring")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult TriggerRecurring()
        {
            RecurringJob.Trigger("obligations-monthly");
            _logger.LogInformation("Recurring job 'obligations-monthly' disparado manualmente");
            return NoContent();
        }

        /// <summary>
        /// Marca la obligación mensual como pagada (Status = PAID, Locked = true).
        /// </summary>
        [HttpPost("{id:int}/pay")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Pay(int id)
        {
            await _svc.MarkAsPaidAsync(id);
            return NoContent();
        }

        // ------------------------ Helpers ------------------------
        private (int year, int month) ResolvePeriod(int? year, int? month)
        {
            if (year.HasValue && month.HasValue) return (year.Value, month.Value);

            var tzId = _cfg["Hangfire:TimeZoneIana"] ?? "America/Bogota";
            var tz = TZConvert.GetTimeZoneInfo(tzId);
            var nowLocal = TimeZoneInfo.ConvertTime(DateTime.UtcNow, tz);

            return (year ?? nowLocal.Year, month ?? nowLocal.Month);
        }
    }
}
