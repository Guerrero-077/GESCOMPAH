using Entity.DTOs.Implements.Business.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces.PDF
{
    public interface IContractPdfGeneratorService
    {
        Task<byte[]> GeneratePdfAsync(ContractSelectDto contract);
    }
}
    