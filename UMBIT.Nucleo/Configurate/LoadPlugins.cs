using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using UMBIT.Nucleo.Conventions;

namespace UMBIT.Nucleo.Configurate
{
    public static class LoadPlugins
    {
        public static string PathPlugins = "UMBIT_PLUGINS";

        public static void AddPluginsMVC(this IServiceCollection services, IWebHostEnvironment _hostingEnvironment)
        {
            var modulesInfo = CarregaPlugins(_hostingEnvironment, GetModules());

            var mvcBuilder = services.AddMvc();

            var moduleInitializerInterface = typeof(IModuleInitializer);

            foreach (var module in modulesInfo)
            {
                // Registra controller dos plugins
                mvcBuilder.AddApplicationPart(module.Assembly).AddMvcOptions(m =>
                {
                    m.Conventions.Add(new ControllerConventions());
                });

                // Registra as depedencias dos plugins
                var moduleInitializerType = module.Assembly.GetTypes().Where(x => typeof(IModuleInitializer).IsAssignableFrom(x)).FirstOrDefault();
                if (moduleInitializerType != null && moduleInitializerType != typeof(IModuleInitializer))
                {
                    var moduleInitializer = (IModuleInitializer)Activator.CreateInstance(moduleInitializerType);
                    moduleInitializer.Init(services);
                }
            }
        }

        private static IList<ModuleInfo> CarregaPlugins(IWebHostEnvironment _hostingEnvironment, IList<ModuleInfo> modules)
        {

            var binFolder = new DirectoryInfo(Environment.GetEnvironmentVariable(PathPlugins));
            if (!binFolder.Exists)
            {
                return null;
            }

            foreach (var file in binFolder.GetFileSystemInfos("*.dll", SearchOption.AllDirectories))
            {
                Assembly assembly;
                try
                {
                    assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file.FullName);
                }
                catch (FileLoadException ex)
                {
                    if (ex.Message == "Assembly with same name is already loaded")
                    {
                        continue;
                    }
                    throw;
                }

                modules.Add(new ModuleInfo { Name = file.Name, Assembly = assembly, Path = file.FullName });

            }

            return modules;
        }



        private static IList<ModuleInfo> GetModules()
        {
            return new List<ModuleInfo>();
        }
    }

}
