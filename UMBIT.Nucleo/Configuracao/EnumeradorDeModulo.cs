using UMBIT.MVC.Core.Configurate.LoadPluginsConfigurate.Initializable.Module;

namespace UMBIT.Nucleo.Core.Configuracao
{
    public class EnumeradorDeModulo : ModuloInfo
    {
        public EnumeradorDeModulo(int identificador, string descricao, string icone, params RecursoInfo[] recursos) : base(identificador, descricao, icone, recursos)
        {
        }
    }
}
