using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace UMBIT.Nucleo.Configurate.Initializable
{

    public interface IModuleInitializer
    {
        void Init(IServiceCollection serviceCollection, IWebHostEnvironment webHostEnvironment);
    }

}