using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace WebGESCOMPAH.Extensions
{
    public static class DatabaseService
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(opciones => opciones
              .UseSqlServer("name=DefaultConnection"));

            return services;
        }
    }
}