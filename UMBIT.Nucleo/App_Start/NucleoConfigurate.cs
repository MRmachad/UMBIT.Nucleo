using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UMBIT.Nucleo.Core.Configurate.InfraConfigurate;

namespace UMBIT.Nucleo.Core.Configurate
{
    public static class NucleoConfigurate
    {
        public static void AddNucleoConfigurate(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddPluginsMVC(configuration);

            services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
               .AddNegotiate();

            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = options.DefaultPolicy;
            });
            services.AddRazorPages();

            services.AddTSEDataBase(configuration, services.BuildServiceProvider());
        }
        public static void UseNucleoConfigurate(this WebApplication app, IWebHostEnvironment webHostEnvironment)
        {
            if (!webHostEnvironment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UsePluginsRoot(app.Services);

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            

            app.UseEndpoints(delegate (IEndpointRouteBuilder endpoints)
            {

                endpoints.MapControllerRoute(
                            name: "module",
                            pattern: "{module}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                            name: "default",
                            pattern: "{controller=Home}/{action=Index}/{id?}");
            });

           
            app.UseMigrate();

        }
        
    }
}
