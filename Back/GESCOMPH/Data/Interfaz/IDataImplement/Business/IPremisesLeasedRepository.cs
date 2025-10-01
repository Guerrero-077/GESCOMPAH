﻿using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Business;

namespace Data.Interfaz.IDataImplement.Business
{
    public interface IPremisesLeasedRepository : IDataGeneric<PremisesLeased>
    {
        Task AddRangeAsync(IEnumerable<PremisesLeased> entities);

    }
}
