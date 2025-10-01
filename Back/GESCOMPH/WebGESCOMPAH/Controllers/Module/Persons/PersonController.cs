using Business.Interfaces.Implements.Persons;
using Entity.DTOs.Implements.Persons.Person;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPH.Controllers.Base;

namespace WebGESCOMPH.Controllers.Module.Persons
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PersonController : BaseController<PersonSelectDto, PersonDto, PersonUpdateDto>
    {
        private readonly IPersonService personService;
        public PersonController(IPersonService service, ILogger<PersonController> logger) : base(service, logger)
        {
            personService = service;
        }


        /// <summary>
        /// Busca una persona por su número de documento.
        /// Retorna también su correo electrónico si está vinculada a un usuario.
        /// </summary>
        /// <param name="document">Número de documento</param>
        /// <returns>Datos de la persona</returns>
        [HttpGet("document/{document}")]
        [AllowAnonymous] // ⬅️ Opcional: según tu seguridad
        public async Task<ActionResult<PersonSelectDto>> GetByDocument(string document)
        {
            var result = await personService.GetByDocumentAsync(document);

            if (result == null)
                return NotFound($"No se encontró una persona con documento: {document}");

            return Ok(result);
        }

    }
}
