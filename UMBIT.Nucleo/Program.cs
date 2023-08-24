using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using UMBIT.Nucleo.Core.Configurate;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddNucleoConfigurate(builder.Configuration);

        var app = builder.Build();

        app.UseNucleoConfigurate(app.Environment);
        app.Run();
    }
}