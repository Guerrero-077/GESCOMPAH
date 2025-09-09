using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.Business.ObligationMonth;

namespace Business.Interfaces.Implements.Business
{
    public interface IObligationMonthService : IBusiness<ObligationMonthSelectDto, ObligationMonthDto, ObligationMonthUpdateDto>
    {
        Task GenerateMonthlyAsync(int year, int month);
        Task MarkAsPaidAsync(int id);
    }
}
