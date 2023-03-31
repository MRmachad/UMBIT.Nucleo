using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UMBIT.Nucleo.Configurate.LoadPluginsConfigurate;

namespace UMBIT.Nucleo.Configurate
{
    public static class NucleoConfigurate
    {
        public static void AddNucleoConfigurate(this IServiceCollection services, IWebHostEnvironment webHostEnvironment)
        {
            services.AddPluginsMVC(webHostEnvironment);

            services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
               .AddNegotiate();

            services.AddAuthorization(options =>
            {
                // By default, all incoming requests will be authorized according to the default policy.
                options.FallbackPolicy = options.DefaultPolicy;
            });
            services.AddRazorPages();
        }
        public static void UseNucleoConfigurate(this WebApplication app, IWebHostEnvironment webHostEnvironment)
        {
            // Configure the HTTP request pipeline.
            if (!webHostEnvironment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UsePluginsRoot();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            

            app.UseEndpoints(delegate (IEndpointRouteBuilder endpoints)
            {
                endpoints.MapControllerRoute(
                            name : "modulo", 
                            pattern :"{modulo}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                            name: "default",
                            pattern: "{controller=Home}/{action=Index}/{id?}");
            });
           


        }

    }
}
