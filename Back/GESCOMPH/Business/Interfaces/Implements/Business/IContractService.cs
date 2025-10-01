using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Business.Interfaces.IBusiness;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.Contract;
using Entity.DTOs.Implements.Business.ObligationMonth;

namespace Business.Interfaces.Implements.Business
{
    public interface IContractService
        : IBusiness<ContractSelectDto, ContractCreateDto, ContractUpdateDto>
    {
        /// <summary>
        /// Crea un contrato, gestionando la creación o reutilización de persona y usuario,
        /// así como la reserva de establecimientos, cláusulas, obligaciones mensuales y notificación por correo.
        /// </summary>
        Task<int> CreateContractWithPersonHandlingAsync(ContractCreateDto dto);

        /// <summary>
        /// Devuelve los contratos en formato de tarjeta visual, según el contexto del usuario actual.
        /// </summary>
        Task<IReadOnlyList<ContractCardDto>> GetMineAsync();

        /// <summary>
        /// Barrido de contratos expirados:
        /// - Marca contratos como inactivos si han expirado.
        /// - Libera los establecimientos asociados.
        /// </summary>
        Task<ExpirationSweepResult> RunExpirationSweepAsync(CancellationToken ct = default);

        /// <summary>
        /// Devuelve las obligaciones mensuales asociadas a un contrato.
        /// </summary>
        Task<IReadOnlyList<ObligationMonthSelectDto>> GetObligationsAsync(int contractId);
    }
}
