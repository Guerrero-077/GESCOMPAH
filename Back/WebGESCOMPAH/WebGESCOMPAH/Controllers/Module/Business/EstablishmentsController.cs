using Business.Interfaces.Implements.Business;
using Entity.DTOs.Implements.Business.Establishment;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebGESCOMPAH.Controllers.Module.Business
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstablishmentsController : ControllerBase
    {
        private readonly IEstablishmentService _establishmentService;
        public EstablishmentsController(IEstablishmentService establishmentService)
        {
            _establishmentService = establishmentService;
            
        }
        // GET: api/<EstablishmentsController>
        [HttpGet]
        public async Task<IEnumerable<EstablishmentSelectDto>> Get()
        {
            return await _establishmentService.GetAllAsync();
        }

        // GET api/<EstablishmentsController>/5
        [HttpGet("{id}")]
        public async Task<EstablishmentSelectDto?> Get(int id)
        {
            return await _establishmentService.GetByIdAsync(id);
        }
    

        // POST api/<EstablishmentsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EstablishmentCreateDto dto)
        {
            var result = await _establishmentService.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        // PUT api/<EstablishmentsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] EstablishmentUpdateDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var result = await _establishmentService.UpdateAsync(dto);
            if (result == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE api/<EstablishmentsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _establishmentService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
