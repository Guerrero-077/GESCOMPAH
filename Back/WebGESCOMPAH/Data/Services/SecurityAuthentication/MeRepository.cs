﻿using Data.Interfaz.Security;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.SecurityAuthentication
{
    public class MeRepository : IUserMeRepository
    {
        private readonly ApplicationDbContext _context;
        public MeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserWithPersonAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Person)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
        }




        public async Task<User?> GetUserWithFullContextAsync(int userId)
        {
            // Trae usuario + persona + roles + rol->rolFormPermissions -> form -> formmodule -> module -> permission
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == userId && !u.IsDeleted && u.Active)
                .Include(u => u.Person)
                .Include(u => u.RolUsers)
                    .ThenInclude(ru => ru.Rol)
                        .ThenInclude(r => r.RolFormPermissions)
                            .ThenInclude(rfp => rfp.Form)
                                .ThenInclude(f => f.FormModules)
                                    .ThenInclude(fm => fm.Module)
                .Include(u => u.RolUsers)
                    .ThenInclude(ru => ru.Rol)
                        .ThenInclude(r => r.RolFormPermissions)
                            .ThenInclude(rfp => rfp.Permission)
                .SingleOrDefaultAsync();
        }



        public async Task<IEnumerable<RolUser>> GetUserRolesWithPermissionsAsync(int userId)
        {
            return await _context.RolUsers
                                .Include(ur => ur.Rol)
                                    .ThenInclude(r => r.RolFormPermissions)
                                        .ThenInclude(rfp => rfp.Permission)
                                .Include(ur => ur.Rol)
                                    .ThenInclude(r => r.RolFormPermissions)
                                        .ThenInclude(rfp => rfp.Form)
                                .Where(ur => ur.UserId == userId && !ur.IsDeleted)
                                .ToListAsync();
        }


        public async Task<IEnumerable<Form>> GetFormsWithModulesByIdsAsync(List<int> formIds)
        {
            return await _context.Forms
                    .Include(f => f.FormModules)
                        .ThenInclude(mf => mf.Module)
                    .Where(f => formIds.Contains(f.Id) && !f.IsDeleted)
                    .ToListAsync();
        }

    }
}
