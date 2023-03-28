using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Embedded;
using Microsoft.Extensions.Hosting;
using UMBIT.Nucleo.Configurate;
using UMBIT.Plugin.Controllers;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddPluginsMVC(builder.Environment);


        builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
           .AddNegotiate();

        builder.Services.AddAuthorization(options =>
        {
            // By default, all incoming requests will be authorized according to the default policy.
            options.FallbackPolicy = options.DefaultPolicy;
        });
        builder.Services.AddRazorPages();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles(new StaticFileOptions()
        {
            FileProvider = new EmbeddedFileProvider(typeof(HomeController).Assembly, typeof(HomeController).Assembly.GetName().Name),
            RequestPath= new PathString("/teste")
        }) ;
        
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseStaticFiles();



        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.Run();
    }
}