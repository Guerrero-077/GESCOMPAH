using Business.Interfaces.Implements.Business;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Data.Interfaz.IDataImplement.Business;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.ObligationMonth;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Utilities.Exceptions;

namespace Business.Services.Business
{
    public class ObligationMonthService : BusinessGeneric<ObligationMonthSelectDto, ObligationMonthDto, ObligationMonthUpdateDto, ObligationMonth>, IObligationMonthService
    {
        private readonly IObligationMonthRepository _obligationRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IDataGeneric<SystemParameter> _systemParamRepository;

        public ObligationMonthService(
            IObligationMonthRepository obligationRepository,
            IContractRepository contractRepository,
            IDataGeneric<SystemParameter> systemParamRepository,
            IMapper mapper) : base(obligationRepository, mapper)
        {
            _obligationRepository = obligationRepository;
            _contractRepository = contractRepository;
            _systemParamRepository = systemParamRepository;
        }

        public async Task GenerateMonthlyAsync(int year, int month)
        {
            var periodDate = new DateTime(year, month, 1);

            var uvtValue = await GetParameterValueAsync("UVT", periodDate);
            var vatRate = await GetParameterValueAsync("IVA", periodDate);

            var contracts = await _contractRepository.GetAllQueryable()
                .Where(c => c.Active && c.StartDate <= periodDate && c.EndDate >= periodDate)
                .ToListAsync();

            foreach (var contract in contracts)
            {
                var existing = await _obligationRepository.GetByContractYearMonthAsync(contract.Id, year, month);
                if (existing != null && existing.Locked)
                    continue;

                var baseAmount = contract.TotalUvtQtyAgreed * uvtValue;
                var vatAmount = baseAmount * vatRate;
                var totalAmount = baseAmount + vatAmount;

                if (existing == null)
                {
                    var obligation = new ObligationMonth
                    {
                        ContractId = contract.Id,
                        Year = year,
                        Month = month,
                        DueDate = periodDate.AddMonths(1).AddDays(-1),
                        UvtQtyApplied = contract.TotalUvtQtyAgreed,
                        UvtValueApplied = uvtValue,
                        VatRateApplied = vatRate,
                        BaseAmount = baseAmount,
                        VatAmount = vatAmount,
                        TotalAmount = totalAmount,
                        Status = "PENDING"
                    };

                    await _obligationRepository.AddAsync(obligation);
                }
                else
                {
                    existing.UvtQtyApplied = contract.TotalUvtQtyAgreed;
                    existing.UvtValueApplied = uvtValue;
                    existing.VatRateApplied = vatRate;
                    existing.BaseAmount = baseAmount;
                    existing.VatAmount = vatAmount;
                    existing.TotalAmount = totalAmount;
                    existing.Status = "PENDING";

                    await _obligationRepository.UpdateAsync(existing);
                }
            }
        }


        public async Task MarkAsPaidAsync(int id)
        {
            // Obtener la obligación existente (sin tracking en Data)
            var existing = await _obligationRepository.GetByIdAsync(id)
                ?? throw new BusinessException($"No existe obligación mensual con Id {id}.");

            // Marcar como pagada y bloquear recálculo
            existing.Status = "PAID";
            existing.Locked = true;

            await _obligationRepository.UpdateAsync(existing);
        }


        private async Task<decimal> GetParameterValueAsync(string key, DateTime date)
        {
            var param = await _systemParamRepository.GetAllQueryable()
                .Where(p => p.Key == key && p.EffectiveFrom <= date && (p.EffectiveTo == null || p.EffectiveTo >= date))
                .OrderByDescending(p => p.EffectiveFrom)
                .FirstOrDefaultAsync();

            if (param == null)
                throw new BusinessException($"Parámetro '{key}' no encontrado para la fecha {date:yyyy-MM-dd}.");

            if (!TryParseDecimalFlexible(param.Value, out var value))
                throw new BusinessException($"Valor inválido para parámetro '{key}': '{param.Value}'. Use 0.19 (o 19) para 19%.");

            // Normaliza IVA si llegó como porcentaje (19 => 0.19)
            if (key.Equals("IVA", StringComparison.OrdinalIgnoreCase))
            {
                if (value >= 1m) value /= 100m;
                if (value < 0m || value > 1m)
                    throw new BusinessException($"El parámetro 'IVA' debe estar entre 0 y 1. Recibido: {value}.");
            }

            // Validaciones básicas
            if (key.Equals("UVT", StringComparison.OrdinalIgnoreCase) && value <= 0m)
                throw new BusinessException("UVT debe ser mayor que 0.");

            return value;
        }

        // Acepta "0.19", "19", "19,00" con punto o coma
        private static bool TryParseDecimalFlexible(string raw, out decimal value)
        {
            raw = raw?.Trim() ?? "";
            if (decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out value)) return true;   // 0.19
            var es = CultureInfo.GetCultureInfo("es-CO");
            if (decimal.TryParse(raw, NumberStyles.Any, es, out value)) return true;                             // 19,00
            return decimal.TryParse(raw, NumberStyles.Any, CultureInfo.CurrentCulture, out value);
        }
    }
}
