using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UMBIT.Infraestrutura.Core.Database.EF;
using UMBIT.Infraestrutura.Core.Interfaces.Database;

namespace UMBIT.Nucleo.Core.Configurate.InfraConfigurate
{
    public static class DatabaseConfigurate
    {
        public static void AddTSEDataBase(this IServiceCollection services, IConfiguration configuration, ServiceProvider serviceProvider)
        {
            StackTrace stackTrace = new StackTrace();

            var conexao = configuration.GetSection("ConnectionString").Value ?? "";

            var nameNucleo = stackTrace.GetFrame(1).GetMethod().DeclaringType.Assembly.GetName().Name;

            services.AddDbContext<DbContext, DataContext>(options =>  options.UseSqlServer(conexao, b =>
            {
                b.MigrationsAssembly(nameNucleo);
            }));

            services.AddScoped<IUnidadeDeTrabalho, UnidadeDeTrabalho>();
        }
        public static void UseMigrate(this WebApplication app)
        {
            using (var serviceScope = app.Services.GetService<IServiceScopeFactory>()?.CreateScope())
            {
                var context = serviceScope?.ServiceProvider.GetRequiredService<DbContext>();

                if (context.Database.GetPendingMigrations().Any())
                {
                    try
                    {
                        context.Database.Migrate();
                    }
                    catch (Exception)
                    {
                        if (!context?.Database.EnsureCreated() ?? false)
                            context.Database.EnsureDeleted();
                        context.Database.Migrate();
                    }

                }
            }
        }
    }
}
