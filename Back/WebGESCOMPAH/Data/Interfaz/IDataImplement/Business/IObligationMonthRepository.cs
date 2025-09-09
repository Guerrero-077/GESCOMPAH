using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Business;

namespace Data.Interfaz.IDataImplement.Business
{
    public interface IObligationMonthRepository : IDataGeneric<ObligationMonth>
    {
        Task<ObligationMonth?> GetByContractYearMonthAsync(int contractId, int year, int month);
        IQueryable<ObligationMonth> GetByContractQueryable(int contractId);
    }
}
