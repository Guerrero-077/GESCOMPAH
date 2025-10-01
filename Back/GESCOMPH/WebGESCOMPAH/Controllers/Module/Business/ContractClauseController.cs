﻿using Business.Interfaces.IBusiness;
using Business.Interfaces.Implements.Business;
using Entity.DTOs.Implements.Business.ContractClause;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPH.Controllers.Base;

namespace WebGESCOMPH.Controllers.Module.Business
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractClauseController : BaseController<ContractClauseSelectDto, ContractClauseDto, ContractClauseUpdateDto>
    {
        public ContractClauseController(IContractClauseService service, ILogger<ContractClauseController> logger) : base(service, logger)
        {
        }
    }
}
