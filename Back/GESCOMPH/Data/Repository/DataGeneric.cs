using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entity.Domain.Models.ModelBase;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository
{
    public class DataGeneric<T> : ADataGenerica<T> where T : BaseModel
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public DataGeneric(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        private IQueryable<T> BaseQuery() =>
            _dbSet.AsNoTracking()
                  .Where(e => !e.IsDeleted)
                  .OrderByDescending(e => e.CreatedAt)
                  .ThenByDescending(e => e.Id);

        // ===== CRUD =====
        public override async Task<IEnumerable<T>> GetAllAsync()
            => await BaseQuery().ToListAsync();

        public override async Task<T?> GetByIdAsync(int id)
            => await _dbSet.AsNoTracking()
                           .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

        public override async Task<T> AddAsync(T entity)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public override async Task<T> UpdateAsync(T entity)
        {
            var existing = await _dbSet.FirstOrDefaultAsync(x => x.Id == entity.Id && !x.IsDeleted);
            if (existing is null)
                throw new InvalidOperationException($"No se encontró entidad con ID {entity.Id}");

            var originalCreatedAt = existing.CreatedAt;
            var originalId = existing.Id;

            _context.Entry(existing).CurrentValues.SetValues(entity);
            existing.Id = originalId;
            existing.CreatedAt = originalCreatedAt;

            await _context.SaveChangesAsync();
            return existing;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return false;
            _dbSet.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public override async Task<bool> DeleteLogicAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return false;

            entity.IsDeleted = true;
            _dbSet.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public override IQueryable<T> GetAllQueryable() => BaseQuery();

    }
}
