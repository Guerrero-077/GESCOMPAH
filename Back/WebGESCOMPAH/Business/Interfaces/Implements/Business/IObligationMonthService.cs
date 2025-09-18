using System.Collections.Generic;
using Business.Interfaces.IBusiness;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.ObligationMonth;

namespace Business.Interfaces.Implements.Business
{
    public interface IObligationMonthService
        : IBusiness<ObligationMonthSelectDto, ObligationMonthDto, ObligationMonthUpdateDto>
    {
        /// <summary>
        /// Genera/actualiza obligaciones para un período (YYYY-MM) para todos los contratos activos ese mes.
        /// Idempotente y pensado para ejecución programada (Hangfire).
        /// </summary>
        Task GenerateMonthlyAsync(int year, int month);

        /// <summary>
        /// Genera/actualiza la obligación de un contrato puntual para un período (YYYY-MM).
        /// No hace nada si el período está fuera del rango del contrato o la obligación está bloqueada.
        /// </summary>
        Task GenerateForContractMonthAsync(int contractId, int year, int month);

        /// <summary>
        /// Obtiene las obligaciones mensuales de un contrato.
        /// </summary>
        Task<IReadOnlyList<ObligationMonthSelectDto>> GetByContractAsync(int contractId);

        /// <summary>
        /// Marca una obligación como pagada y la bloquea.
        /// </summary>
        Task MarkAsPaidAsync(int id);
    }
}
