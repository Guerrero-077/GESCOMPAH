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

        public async Task GenerateMonthlyAsync(int year, int month)
        {
            var (monthStart, monthEnd, dueDate) = GetPeriodDates(year, month);
            var uvtValue = await GetParameterValueAsync("UVT", dueDate);
            var vatRate = await GetParameterValueAsync("IVA", dueDate);

            var contracts = await _contractRepository.GetAllQueryable()
                .Where(c => c.Active && c.StartDate < monthEnd && c.EndDate >= monthStart)
                .ToListAsync();

            foreach (var contract in contracts)
                await UpsertObligationAsync(contract, monthStart, uvtValue, vatRate);
        }

        public async Task GenerateForContractMonthAsync(int contractId, int year, int month)
        {
            var contract = await _contractRepository.GetByIdAsync(contractId)
                ?? throw new BusinessException("Contrato no existe.");

            var (monthStart, monthEnd, dueDate) = GetPeriodDates(year, month);

            if (!(contract.StartDate < monthEnd && contract.EndDate >= monthStart))
                return;

            var uvtValue = await GetParameterValueAsync("UVT", dueDate);
            var vatRate = await GetParameterValueAsync("IVA", dueDate);

            await UpsertObligationAsync(contract, monthStart, uvtValue, vatRate);
        }

        public async Task<IReadOnlyList<ObligationMonthSelectDto>> GetByContractAsync(int contractId)
        {
            if (contractId <= 0)
                throw new BusinessException("contractId invalido.");

            var list = await _obligationRepository.GetByContractQueryable(contractId)
                .AsNoTracking()
                .ToListAsync();

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
                return;

            var (baseAmount, vatAmount, totalAmount) = CalculateAmounts(contract, uvtValue, vatRate);
            var dueDate = new DateTime(periodDate.Year, periodDate.Month, DateTime.DaysInMonth(periodDate.Year, periodDate.Month));

            if (existing == null)
            {
                var obligation = new ObligationMonth
                {
                    ContractId = contract.Id,
                    Year = periodDate.Year,
                    Month = periodDate.Month,
                    DueDate = dueDate,
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

                if (existing.Status == "CANCELLED")
                    existing.Status = "PENDING";

                await _obligationRepository.UpdateAsync(existing);
            }
        }

        private (DateTime MonthStart, DateTime MonthEnd, DateTime DueDate) GetPeriodDates(int year, int month)
        {
            var monthStart = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var monthEnd = monthStart.AddMonths(1);
            var dueDate = new DateTime(year, month, DateTime.DaysInMonth(year, month), 0, 0, 0, DateTimeKind.Utc);
            return (monthStart, monthEnd, dueDate);
        }

        private (decimal BaseAmount, decimal VatAmount, decimal TotalAmount) CalculateAmounts(Contract contract, decimal uvtValue, decimal vatRate)
        {
            decimal baseAmount = contract.TotalBaseRentAgreed > 0m
                ? contract.TotalBaseRentAgreed
                : contract.TotalUvtQtyAgreed * uvtValue;

            decimal vatAmount = baseAmount * vatRate;
            return (baseAmount, vatAmount, baseAmount + vatAmount);
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
                if (value >= 1m) value /= 100m;
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
