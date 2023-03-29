using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
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
                    m.Conventions.Add(new ActionConvetions());
                    m.EnableEndpointRouting = false;

                }).AddViewOptions(options =>
                {
                    //options.ViewEngines.Clear();
                    //options.ViewEngines.Add(new UMBITViewEngine());
                }).AddRazorRuntimeCompilation();

                // Registra as depedencias dos plugins
                var moduleInitializerType = module.Assembly.GetTypes().Where(x => typeof(IModuleInitializer).IsAssignableFrom(x)).FirstOrDefault();
                if (moduleInitializerType != null && moduleInitializerType != typeof(IModuleInitializer))
                {
                    var moduleInitializer = (IModuleInitializer)Activator.CreateInstance(moduleInitializerType);
                    moduleInitializer.Init(services);
                }

                services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
                {
                    options.FileProviders.Add(new UMBITEmbeddedFileProvider(module.Assembly, module.Assembly.GetName().Name));
                });
            }

            services.Configure(delegate (RazorViewEngineOptions options)
            {
                options.ViewLocationExpanders.Add(new UMBITViewLocationExpander());
            });
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

        public static void UsePluginsRoot(this IApplicationBuilder applicationBuilder, IWebHostEnvironment webHostEnvironment)
        {
            var plugins = CarregaPlugins(webHostEnvironment, GetModules());

            var listFileProvider = new List<IFileProvider>();

            foreach (var plugin in plugins)
            {
                applicationBuilder.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = new UMBITEmbeddedFileProvider(plugin.Assembly, plugin.Assembly.GetName().Name),
                    RequestPath = new PathString("/" + plugin.Assembly.GetName().Name),
                });
            }

        }
    }

}
