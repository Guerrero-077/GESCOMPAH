using System.Threading;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IUnitOfWork
    {
        Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken ct = default);
        Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken ct = default);
        void RegisterPostCommit(Func<CancellationToken, Task> action);
    }
}
