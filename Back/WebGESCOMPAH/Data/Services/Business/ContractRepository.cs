using Data.Interfaz.IDataImplement.Business;
using Data.Repository;
using Entity.Domain.Models.Implements.Business;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Utilities.Exceptions;

namespace Data.Services.Business
{
    public class ContractRepository : DataGeneric<Contract>, IContractRepository
    {
        public ContractRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Contract>> GetAllAsync()
        {
            try
            {
                //return await _dbSet
                //    .Include(c => c.Person)
                //    .Include(c => c.PremisesLeased)
                //        .ThenInclude(pl => pl.Establishment)
                //    .Include(c => c.ContractTerms)
                //    .AsNoTracking()
                //    .ToListAsync();

                return await _dbSet
                    .Include(c => c.Person)
                        .ThenInclude(p => p.User)
                    .Include(c => c.PremisesLeased)
                        .ThenInclude(pl => pl.Establishment)
                            .ThenInclude(e => e.Plaza)
                    //.Include(c => c.ContractTerms)
                    .AsNoTracking()
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                throw new DataException("Error al obtener la lista de contratos.", ex);
            }
        }


        public override async Task<Contract?> GetByIdAsync(int id)
        {
            try
            {
                //return await _dbSet
                //    .Include(c => c.Person)
                //    .Include(c => c.PremisesLeased)
                //        .ThenInclude(pl => pl.Establishment)
                //    .Include(c => c.ContractTerms)
                //    .AsNoTracking()
                //    .FirstOrDefaultAsync(c => c.Id == id);

                return await _dbSet
                        .Include(c => c.Person)
                            .ThenInclude(p => p.User) // 🟢 Incluye el usuario vinculado a la persona
                        .Include(c => c.PremisesLeased)
                            .ThenInclude(pl => pl.Establishment)
                                .ThenInclude(e => e.Plaza) // 🟢 Incluye la plaza asociada al local
                        //.Include(c => c.ContractTerms)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                throw new DataException($"Error al obtener el contrato con ID {id}.", ex);
            }
        }

    }
}
