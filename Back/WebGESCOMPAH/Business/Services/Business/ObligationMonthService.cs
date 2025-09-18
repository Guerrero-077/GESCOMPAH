using Business.Interfaces.Implements.Business;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Data.Interfaz.IDataImplement.Business;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.ObligationMonth;
using MapsterMapper;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Utilities.Exceptions;

namespace Business.Services.Business
{
    public class ObligationMonthService
        : BusinessGeneric<ObligationMonthSelectDto, ObligationMonthDto, ObligationMonthUpdateDto, ObligationMonth>,
          IObligationMonthService
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

        /// <summary>
        /// Genera obligaciones para el período indicado (un mes) para todos los contratos que
        /// solapan con ese mes. Idempotente (respeta Locked).
        /// </summary>
        public async Task GenerateMonthlyAsync(int year, int month)
        {
            // Rango del mes [monthStart, monthEnd)
            var monthStart = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var monthEnd = monthStart.AddMonths(1);
            var dueDate = new DateTime(year, month, DateTime.DaysInMonth(year, month), 0, 0, 0, DateTimeKind.Utc);

            var uvtValue = await GetParameterValueAsync("UVT", dueDate);
            var vatRate = await GetParameterValueAsync("IVA", dueDate);

            var contracts = await _contractRepository.GetAllQueryable()
                .Where(c => c.Active
                    && c.StartDate < monthEnd
                    && c.EndDate >= monthStart)
                .ToListAsync();

            foreach (var contract in contracts)
            {
                await UpsertObligationAsync(contract, monthStart, uvtValue, vatRate);
            }
        }

        /// <summary>
        /// Genera/actualiza la obligación de un contrato específico para (year, month).
        /// No backfillea otros meses.
        /// </summary>
        public async Task GenerateForContractMonthAsync(int contractId, int year, int month)
        {
            var contract = await _contractRepository.GetByIdAsync(contractId)
                ?? throw new BusinessException("Contrato no existe.");

            var monthStart = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var monthEnd = monthStart.AddMonths(1);
            var dueDate = new DateTime(year, month, DateTime.DaysInMonth(year, month), 0, 0, 0, DateTimeKind.Utc);

            // Contrato vigente en cualquier día del mes
            var overlaps = contract.StartDate < monthEnd && contract.EndDate >= monthStart;
            if (!overlaps) return;

            var uvtValue = await GetParameterValueAsync("UVT", dueDate);
            var vatRate = await GetParameterValueAsync("IVA", dueDate);

            await UpsertObligationAsync(contract, monthStart, uvtValue, vatRate);
        }

        public async Task<IReadOnlyList<ObligationMonthSelectDto>> GetByContractAsync(int contractId)
        {
            if (contractId <= 0)
                throw new BusinessException("contractId invalido.");

            var query = _obligationRepository.GetByContractQueryable(contractId);
            var list = await query.AsNoTracking().ToListAsync();
            return _mapper.Map<List<ObligationMonthSelectDto>>(list).AsReadOnly();
        }

        public async Task MarkAsPaidAsync(int id)
        {
            var existing = await _obligationRepository.GetByIdAsync(id)
                ?? throw new BusinessException($"No existe obligación mensual con Id {id}.");

            existing.Status = "PAID";
            existing.Locked = true;

            await _obligationRepository.UpdateAsync(existing);
        }

        // ------------------ Helpers internos ------------------

        private async Task UpsertObligationAsync(Contract contract, DateTime periodDate, decimal uvtValue, decimal vatRate)
        {
            var existing = await _obligationRepository
                .GetByContractYearMonthAsync(contract.Id, periodDate.Year, periodDate.Month);

            if (existing != null && existing.Locked)
                return; // no tocar obligaciones cerradas

            // ---------- Cálculo (BASE o UVT) ----------
            // Regla: si hay Base pactada (>0) usamos BASE; si no, usamos UVT.
            decimal uvtQtyApplied = contract.TotalUvtQtyAgreed; // snapshot
            decimal baseAmount;

            if (contract.TotalBaseRentAgreed > 0m)
            {
                baseAmount = contract.TotalBaseRentAgreed;              // Estrategia BASE
            }
            else
            {
                baseAmount = uvtQtyApplied * uvtValue;                  // Estrategia UVT
            }

            var vatAmount = baseAmount * vatRate;
            var totalAmount = baseAmount + vatAmount;

            // DueDate: último día del mes (puede parametrizarse)
            var daysInMonth = DateTime.DaysInMonth(periodDate.Year, periodDate.Month);
            var dueDate = new DateTime(periodDate.Year, periodDate.Month, daysInMonth);

            if (existing == null)
            {
                var obligation = new ObligationMonth
                {
                    ContractId = contract.Id,
                    Year = periodDate.Year,
                    Month = periodDate.Month,
                    DueDate = dueDate,
                    UvtQtyApplied = uvtQtyApplied,
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
                existing.UvtQtyApplied = uvtQtyApplied;
                existing.UvtValueApplied = uvtValue;
                existing.VatRateApplied = vatRate;
                existing.BaseAmount = baseAmount;
                existing.VatAmount = vatAmount;
                existing.TotalAmount = totalAmount;

                // Si estaba CANCELLED y vuelve a quedar en rango, reabrimos
                if (existing.Status == "CANCELLED")
                    existing.Status = "PENDING";

                await _obligationRepository.UpdateAsync(existing);
            }
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
                throw new BusinessException($"Valor inválido para parámetro '{key}': '{param.Value}'.");

            if (key.Equals("IVA", StringComparison.OrdinalIgnoreCase))
            {
                if (value >= 1m) value /= 100m; // 19 -> 0.19
                if (value < 0m || value > 1m)
                    throw new BusinessException($"El parámetro 'IVA' debe estar entre 0 y 1. Recibido: {value}.");
            }

            if (key.Equals("UVT", StringComparison.OrdinalIgnoreCase) && value <= 0m)
                throw new BusinessException("UVT debe ser mayor que 0.");

            return value;
        }

        private static bool TryParseDecimalFlexible(string raw, out decimal value)
        {
            raw = raw?.Trim() ?? "";
            if (decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out value)) return true;
            var es = CultureInfo.GetCultureInfo("es-CO");
            if (decimal.TryParse(raw, NumberStyles.Any, es, out value)) return true;
            return decimal.TryParse(raw, NumberStyles.Any, CultureInfo.CurrentCulture, out value);
        }
    }
}



