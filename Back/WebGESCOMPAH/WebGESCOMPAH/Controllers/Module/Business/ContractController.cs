using Business.Interfaces.Implements.Business;
using Business.Interfaces.PDF;
using Entity.DTOs.Implements.Business.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using WebGESCOMPAH.Contracts.Requests;
using WebGESCOMPAH.RealTime;

namespace WebGESCOMPAH.Controllers.Module.Business
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;
        private readonly IContractPdfGeneratorService _pdfService;
        private readonly IHubContext<ContractsHub> _hub;
        private readonly ILogger<ContractController> _logger;

        public ContractController(
            IContractService contractService,
            IContractPdfGeneratorService pdfService,
            ILogger<ContractController> logger,
            IHubContext<ContractsHub> hub)
        {
            _contractService = contractService;
            _pdfService = pdfService;
            _logger = logger;
            _hub = hub;
        }

        [HttpGet("mine")]
        [ProducesResponseType(typeof(IEnumerable<ContractCardDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMine()
        {
            var result = await _contractService.GetMineAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ContractSelectDto>> Post([FromBody] ContractCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var contractId = await _contractService.CreateContractWithPersonHandlingAsync(dto);

                await _hub.Clients.All.SendAsync("contracts:mutated", new
                {
                    type = "created",
                    id = contractId,
                    at = DateTime.UtcNow
                });

                return CreatedAtAction(nameof(GetById), new { id = contractId }, new { contractId });
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning("Error de negocio: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear contrato");
                return StatusCode(500, new { error = "Error interno del servidor." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var contract = await _contractService.GetByIdAsync(id);
            if (contract == null) return NotFound();

            return Ok(contract);
        }

        [HttpPatch("{id:int}/estado")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeActiveStatus(int id, [FromBody] ChangeActiveStatusRequest body)
        {
            try
            {
                await _contractService.UpdateActiveStatusAsync(id, body.Active!.Value);

                await _hub.Clients.All.SendAsync("contracts:mutated", new
                {
                    type = "statusChanged",
                    id,
                    active = body.Active!.Value,
                    at = DateTime.UtcNow
                });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando el estado del contrato {Id}", id);
                return StatusCode(500, new { error = "Error interno del servidor." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var contract = await _contractService.GetByIdAsync(id);
                if (contract == null)
                    return NotFound();

                await _contractService.DeleteAsync(id); // suponiendo que el service tiene DeleteAsync

                await _hub.Clients.All.SendAsync("contracts:mutated", new
                {
                    type = "deleted",
                    id,
                    at = DateTime.UtcNow
                });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error borrando contrato {Id}", id);
                return StatusCode(500, new { error = "Error interno del servidor." });
            }
        }

        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> DownloadContractPdf(int id)
        {
            var contract = await _contractService.GetByIdAsync(id);
            if (contract == null)
            {
                _logger.LogWarning("Contrato con ID {Id} no encontrado.", id);
                return NotFound(new { message = $"No se encontró un contrato con ID {id}" });
            }

            try
            {
                var pdfBytes = await _pdfService.GeneratePdfAsync(contract);
                return File(pdfBytes, "application/pdf", $"Contrato_{contract.FullName}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando PDF para contrato {Id}", id);

                var debug = HttpContext.Request.Query.TryGetValue("debug", out var dv) && dv.ToString() == "1";
                if (debug)
                {
                    return StatusCode(500, new
                    {
                        error = "Error generando PDF",
                        message = ex.Message,
                        inner = ex.InnerException?.Message,
                        stack = ex.StackTrace,
                    });
                }

                return StatusCode(500, new { error = "Error interno al generar el PDF." });
            }
        }

        [HttpGet("{id:int}/obligations")]
        [ProducesResponseType(typeof(IEnumerable<Entity.DTOs.Implements.Business.ObligationMonth.ObligationMonthSelectDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetObligations(int id)
        {
            var contract = await _contractService.GetByIdAsync(id);
            if (contract == null) return NotFound();

            var obligations = await _contractService.GetObligationsAsync(id);
            return Ok(obligations);
        }

        [HttpPost("expire/run")]
        public async Task<IActionResult> RunExpirationNow(CancellationToken ct)
        {
            await _contractService.RunExpirationSweepAsync(ct);
            await _hub.Clients.All.SendAsync("contracts:expired", new { at = DateTime.UtcNow }, ct);
            return NoContent();
        }
    }
}
