using System.Reflection;

namespace UMBIT.Nucleo.Configurate
{
    internal class ModuleInfo
    {
        public string Name { get; internal set; }
        public Assembly Assembly { get; internal set; }
        public string Path { get; internal set; }
    }
}