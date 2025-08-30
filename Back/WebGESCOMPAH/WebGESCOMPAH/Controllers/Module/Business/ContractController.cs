using Business.Interfaces.Implements.Business;
using Business.Interfaces.PDF;
using Entity.DTOs.Implements.Business.Contract;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;
using WebGESCOMPAH.Controllers.Base;

namespace WebGESCOMPAH.Controllers.Module.Business
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : BaseController<ContractSelectDto, ContractCreateDto, ContractUpdateDto>
    {
        private readonly IContractService _contractService;
        private readonly IContractPdfGeneratorService _pdfService;
        public ContractController(IContractService service, IContractPdfGeneratorService pdfService, ILogger<ContractController> logger) : base(service, logger)
        {
            _contractService = service;
            _pdfService = pdfService;
        }



        [HttpGet("mine")]
        [ProducesResponseType(typeof(IEnumerable<ContractCardDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMine() => Ok(await _contractService.GetMineAsync());


        /// <summary>
        /// Crea un nuevo contrato asociado a una persona y usuario (si aplica)
        /// </summary>
        [HttpPost]
        public override async Task<ActionResult<ContractSelectDto>> Post([FromBody] ContractCreateDto dto)

        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var contractId = await _contractService.CreateContractWithPersonHandlingAsync(dto);
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



    }
}
