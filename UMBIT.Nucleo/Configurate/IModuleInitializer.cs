using Microsoft.Extensions.DependencyInjection;

namespace UMBIT.Nucleo.Configurate
{

    public interface IModuleInitializer
    {
        void Init(IServiceCollection serviceCollection);
    }

}