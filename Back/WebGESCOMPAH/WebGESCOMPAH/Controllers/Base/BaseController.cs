using Business.Interfaces.IBusiness;
using Entity.DTOs.Base;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPAH.Contracts.Requests;

namespace WebGESCOMPAH.Controllers.Base
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public abstract class BaseController<TGet, TCreate, TUpdate> : ControllerBase
        where TGet : class
    {
        protected readonly IBusiness<TGet, TCreate, TUpdate> Service;
        protected readonly ILogger Logger;

        protected BaseController(IBusiness<TGet, TCreate, TUpdate> service, ILogger logger)
        {
            Service = service;
            Logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual async Task<ActionResult<IEnumerable<TGet>>> Get()
            => Ok(await Service.GetAllAsync());

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<TGet>> GetById(int id)
            => (await Service.GetByIdAsync(id)) is { } dto ? Ok(dto) : NotFound();

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<ActionResult<TGet>> Post([FromBody] TCreate dto)
        {
            var created = await Service.CreateAsync(dto);

            // Intentar obtener el Id de forma flexible (no todos los DTOs heredan BaseDto)
            int? id = null;

            if (created is BaseDto withId)
            {
                id = withId.Id;
            }
            else
            {
                var prop = created?.GetType().GetProperty("Id");
                if (prop != null && prop.PropertyType == typeof(int))
                {
                    id = (int?)prop.GetValue(created!);
                }
            }

            if (id.HasValue && id.Value > 0)
            {
                // Respuesta 201 con Location al recurso
                return CreatedAtAction(nameof(GetById), new { id = id.Value }, created);
            }

            // Si no hay Id, devolvemos 200 OK con el payload creado
            return Ok(created);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<TGet>> Put(int id, [FromBody] TUpdate dto)
        {
            if (dto is BaseDto withId && withId.Id != 0 && withId.Id != id)
                return BadRequest("El ID del cuerpo no coincide con el ID de la URL.");

            if (dto is BaseDto baseDto)
                baseDto.Id = id;

            var updated = await Service.UpdateAsync(dto);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<IActionResult> Delete(int id)
            => await Service.DeleteAsync(id) ? NoContent() : NotFound();

        [HttpPatch("{id:int}/soft-delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<IActionResult> DeleteLogic(int id)
            => await Service.DeleteLogicAsync(id) ? NoContent() : NotFound();

        [HttpPatch("{id:int}/estado")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public virtual async Task<IActionResult> ChangeActiveStatus(int id, [FromBody] ChangeActiveStatusRequest body)
        {
            if (body.Active is null)
                return BadRequest("El campo 'Active' no puede ser nulo.");

            await Service.UpdateActiveStatusAsync(id, body.Active.Value);
            return NoContent();
        }

        [HttpGet("query")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual async Task<ActionResult<PagedResult<TGet>>> Query(
            [FromQuery] int Page = 1,
            [FromQuery] int Size = 20,
            [FromQuery] string? Search = null,
            [FromQuery] string? Sort = null,
            [FromQuery] bool Desc = true,
            [FromQuery] IDictionary<string, string>? Filters = null)
        {
            if (Page <= 0 || Size <= 0)
                return BadRequest("Los valores de paginación deben ser mayores a cero.");

            var query = new PageQuery(Page, Size, Search, Sort, Desc, Filters);
            var result = await Service.QueryAsync(query);

            // Headers para UI/paginación
            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();
            Response.Headers["X-Page"] = result.Page.ToString();
            Response.Headers["X-Size"] = result.Size.ToString();

            return Ok(result);
        }
    }
}
