using Business.Interfaces.Implements.Persons;
using Entity.DTOs.Implements.Persons.Peron;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPAH.Controllers.Base;

namespace WebGESCOMPAH.Controllers.Module.Persons
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PersonController : BaseController<PersonSelectDto, PersonDto, PersonUpdateDto, IPersonService>
    {
        public PersonController(IPersonService service, ILogger logger) : base(service, logger)
        {
        }

        protected override async Task<IEnumerable<PersonSelectDto>> GetAllAsync()
        {
            return await _service.GetAllAsync();
        }

        protected override async Task<PersonSelectDto?> GetByIdAsync(int id)
        {
            return await _service.GetByIdAsync(id);
        }
        protected override async Task AddAsync(PersonDto dto)
        {
            await _service.CreateAsync(dto);
        }
     

        protected override Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        protected override Task<bool> DeleteLogicAsync(int id)
        {
            throw new NotImplementedException();
        }



        protected override async Task<PersonUpdateDto> UpdateAsync(int id, PersonUpdateDto dto)
        {
            return await _service.UpdateAsync(dto);
        }
    }
}
