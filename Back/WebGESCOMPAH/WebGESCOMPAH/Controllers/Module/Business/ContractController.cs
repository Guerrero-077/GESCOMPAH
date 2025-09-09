// WebGESCOMPAH.Controllers.Module.Business/ContractController.cs
using Business.Interfaces.Implements.Business;
using Business.Interfaces.PDF;
using Entity.DTOs.Implements.Business.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Utilities.Exceptions;
using WebGESCOMPAH.Contracts.Requests;
using WebGESCOMPAH.Controllers.Base;
using WebGESCOMPAH.RealTime;

namespace WebGESCOMPAH.Controllers.Module.Business
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : BaseController<ContractSelectDto, ContractCreateDto, ContractUpdateDto>
    {
        private readonly IContractService _contractService;
        private readonly IContractPdfGeneratorService _pdfService;
        private readonly IHubContext<ContractsHub> _hub; // <-- OPCIONAL: emitir eventos por cambios vía API

        public ContractController(
            IContractService service,
            IContractPdfGeneratorService pdfService,
            ILogger<ContractController> logger,
            IHubContext<ContractsHub> hub) // <-- inyecta hub
            : base(service, logger)
        {
            _contractService = service;
            _pdfService = pdfService;
            _hub = hub;
        }

        [HttpGet("mine")]
        [ProducesResponseType(typeof(IEnumerable<ContractCardDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMine() => Ok(await _contractService.GetMineAsync());

        /// <summary>Crea un nuevo contrato.</summary>
        [HttpPost]
        public override async Task<ActionResult<ContractSelectDto>> Post([FromBody] ContractCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var contractId = await _contractService.CreateContractWithPersonHandlingAsync(dto);

                // 🔔 Notificar a la UI que hubo una mutación por API (opcional)
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
                Logger.LogWarning("Error de negocio: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error inesperado al crear contrato");
                return StatusCode(500, new { error = "Error interno del servidor." });
            }
        }

        [HttpPatch("{id:int}/estado")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<IActionResult> ChangeActiveStatus(int id, [FromBody] ChangeActiveStatusRequest body)
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

        [HttpDelete("{id}")]
        public override async Task<IActionResult> Delete(int id)
        {
            try
            {
                await base.Delete(id); // llama al BaseController o tu lógica de borrado

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
                Logger.LogError(ex, "Error borrando contrato {Id}", id);
                return StatusCode(500, new { error = "Error interno del servidor." });
            }
        }

        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> DownloadContractPdf(int id)
        {
            var contract = await _contractService.GetByIdAsync(id);
            if (contract == null)
            {
                Logger.LogWarning("Contrato con ID {Id} no encontrado.", id);
                return NotFound(new { message = $"No se encontró un contrato con ID {id}" });
            }

            var pdfBytes = await _pdfService.GeneratePdfAsync(contract);
            return File(pdfBytes, "application/pdf", $"Contrato_{contract.FullName}.pdf");
        }

        /// <summary>
        /// Obtiene las obligaciones mensuales de un contrato (ordenadas desc por año/mes).
        /// </summary>
        [HttpGet("{id:int}/obligations")]
        [ProducesResponseType(typeof(IEnumerable<Entity.DTOs.Implements.Business.ObligationMonth.ObligationMonthSelectDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetObligations(int id)
        {
            // Opcional: validar existencia del contrato primero si se desea 404 cuando no existe
            var contract = await _contractService.GetByIdAsync(id);
            if (contract == null) return NotFound();

            var obligations = await _contractService.GetObligationsAsync(id);
            return Ok(obligations);
        }

        // (Opcional) endpoint manual para forzar el barrido y notificar:
        [HttpPost("expire/run")]
        public async Task<IActionResult> RunExpirationNow(CancellationToken ct)
        {
            await _contractService.RunExpirationSweepAsync(ct);
            await _hub.Clients.All.SendAsync("contracts:expired", new { at = DateTime.UtcNow }, ct);
            return NoContent();
        }
    }
}
