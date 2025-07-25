﻿using Business.CQRS.Business.Establishments.Create;
using Business.CQRS.Business.Establishments.Delete;
using Business.CQRS.Business.Establishments.Select;
using Business.CQRS.Business.Establishments.Update;
using Business.Interfaces.Implements.Business;
using Entity.DTOs.Implements.Business.Establishment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]

[Route("api/[controller]")]
public class EstablishmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IEstablishmentService _establishmentService;
    public EstablishmentsController(IMediator mediator, IEstablishmentService establishmentService)
    {
        _mediator = mediator;
        _establishmentService = establishmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllEstablishmentsQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetEstablishmentByIdQuery(id));
        if (result == null)
            return NotFound();
        return Ok(result);
    }
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] EstablishmentCreateDto dto)
    {
        var result = await _mediator.Send(new CreateEstablishmentCommand(dto));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("Update")]
    public async Task<IActionResult> Update([FromForm] EstablishmentUpdateDto dto)
    {
        var result = await _mediator.Send(new UpdateEstablishmentCommand(dto));
        return Ok(result);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, [FromQuery] bool forceDelete = false)
    {
        await _mediator.Send(new DeleteEstablishmentCommand(id, forceDelete));
        return NoContent();
    }


    /// <summary>
    /// Elimina una imagen específica de un establecimiento
    /// </summary>
    [HttpDelete("{establishmentId}/images/{imageId}")]
    public async Task<IActionResult> DeleteImage(int establishmentId, int imageId)
    {
        await _establishmentService.DeleteImageAsync(establishmentId, imageId);
        return NoContent(); // 204
    }
}
