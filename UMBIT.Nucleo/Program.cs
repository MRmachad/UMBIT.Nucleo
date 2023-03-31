using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using UMBIT.Nucleo.Configurate;
using UMBIT.Nucleo.Configurate.LoadPluginsConfigurate;
using UMBIT.Nucleo.Controllers;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddNucleoConfigurate(builder.Environment);

        var app = builder.Build();

        app.UseNucleoConfigurate(app.Environment);
        app.Run();
    }
}