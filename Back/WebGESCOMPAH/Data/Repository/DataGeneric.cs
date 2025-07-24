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

        public override async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.Where(e => !e.IsDeleted).ToListAsync();
        }

        public override async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public override async Task<T> AddAsync(T entity)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public override async Task<T> UpdateAsync(T entity)
        {
            var existingEntity = await _dbSet.FindAsync(entity.Id);
            if (existingEntity == null)
                throw new InvalidOperationException($"No se encontró entidad con ID {entity.Id}");

            _context.Entry(existingEntity).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return existingEntity;
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

        public override IQueryable<T> GetAllQueryable() => _dbSet.AsQueryable();

    }

}
