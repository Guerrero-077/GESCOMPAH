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
        public virtual async Task<ActionResult<TGet>> Post([FromBody] TCreate dto)
        {
            var created = await Service.CreateAsync(dto);
            if (created is BaseDto withId)
                return CreatedAtAction(nameof(GetById), new { id = withId.Id }, created);

            return StatusCode(StatusCodes.Status201Created, created);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<TGet>> Put(int id, [FromBody] TUpdate dto)
        {
            if (dto is BaseDto withId) withId.Id = id;
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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeActiveStatus(
            int id,
            [FromBody] ChangeActiveStatusRequest body)
        {
            await Service.UpdateActiveStatusAsync(id, body.Active!.Value);
            return NoContent(); // 204
        }


        //public virtual async Task<ActionResult<TGet>> ChangeActiveStatus(
        //    int id, [FromBody] ChangeActiveStatusDto body)
        //{
        //    var updated = await Service.UpdateActiveStatusAsync(id, body.Active );
        //    return updated is null ? NotFound() : Ok(updated);
        //}
    }
}
