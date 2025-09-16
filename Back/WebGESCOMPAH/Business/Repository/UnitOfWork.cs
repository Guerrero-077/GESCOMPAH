using Business.Interfaces;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Business.Repository
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UnitOfWork>? _logger;
        private readonly List<Func<CancellationToken, Task>> _postCommitActions = new();

        public UnitOfWork(ApplicationDbContext context, ILogger<UnitOfWork>? logger = null)
        {
            _context = context;
            _logger = logger;
        }

        public async Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken ct = default)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var tx = await _context.Database.BeginTransactionAsync(ct);
                try
                {
                    await action(ct);
                    await tx.CommitAsync(ct);
                    await RunPostCommitAsync(ct);
                }
                catch
                {
                    _postCommitActions.Clear();
                    await tx.RollbackAsync(ct);
                    throw;
                }
            });
        }

        public async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken ct = default)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                await using var tx = await _context.Database.BeginTransactionAsync(ct);
                try
                {
                    var result = await action(ct);
                    await tx.CommitAsync(ct);
                    await RunPostCommitAsync(ct);
                    return result;
                }
                catch
                {
                    _postCommitActions.Clear();
                    await tx.RollbackAsync(ct);
                    throw;
                }
            });
        }

        public void RegisterPostCommit(Func<CancellationToken, Task> action)
        {
            if (action is null) return;
            _postCommitActions.Add(action);
        }

        private async Task RunPostCommitAsync(CancellationToken ct)
        {
            if (_postCommitActions.Count == 0) return;
            var actions = _postCommitActions.ToArray();
            _postCommitActions.Clear();
            foreach (var act in actions)
            {
                try { await act(ct); }
                catch (Exception ex) { _logger?.LogError(ex, "Post-commit action failed"); }
            }
        }
    }
}
