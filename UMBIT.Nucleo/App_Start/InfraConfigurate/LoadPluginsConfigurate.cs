using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UMBIT.Infraestrutura.Core.Entidade;
using UMBIT.MVC.Core.Configurate.FileProvider;
using UMBIT.MVC.Core.Configurate.LoadPluginsConfigurate.Initializable;
using UMBIT.MVC.Core.Configurate.LoadPluginsConfigurate.Initializable.Module;
using UMBIT.MVC.Core.Configurate.LocationExpander;
using UMBIT.MVC.Core.Conventions;

namespace UMBIT.Nucleo.Core.Configurate.InfraConfigurate
{
    public static class LoadPluginsConfigurate
    {


        public static void AddPluginsMVC(this IServiceCollection services, IConfiguration configuration)
        {

            var PluginsInfo = services.LoadDomains(configuration);

            var mvcBuilder = services.AddMvc();


            foreach (var plugin in PluginsInfo.Plugins)
            {
                var MVC = plugin.LoadAssembly(plugin.ProjetoUIWebPath);

                object value = mvcBuilder.AddApplicationPart(MVC).AddMvcOptions(m =>
                {
                    m.Conventions.Add(new ControllerConventions(plugin.Area, MVC.GetName().Name));
                    m.Conventions.Add(new ActionConvetions());
                    m.EnableEndpointRouting = false;

                }).AddRazorRuntimeCompilation();

                var moduleInitializerType = MVC.GetTypes().Where(x => typeof(IPluginInitializer).IsAssignableFrom(x)).FirstOrDefault();
                if (moduleInitializerType != null && moduleInitializerType != typeof(IPluginInitializer))
                {
                    var moduleInitializer = (IPluginInitializer)Activator.CreateInstance(moduleInitializerType);
                    moduleInitializer.Init(services);
                }

                services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
                {
                    options.FileProviders.Add(new UMBITEmbeddedFileProvider(MVC));
                });
            }

            services.Configure(delegate (RazorViewEngineOptions options)
            {
                options.ViewLocationExpanders.Add(new UMBITViewLocationExpander());
            });

        }

        public static void UsePluginsRoot(this IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
        {
            applicationBuilder.UseStaticFiles();

            var pluginsInfo = serviceProvider.GetRequiredService<PluginsInfo>();

            foreach (var plugin in pluginsInfo.Plugins)
            {
                var MVC = plugin.LoadAssembly(plugin.ProjetoUIWebPath);

                applicationBuilder.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = new UMBITEmbeddedFileProviderStatic(MVC),
                    RequestPath = new PathString("/Content/" + MVC.GetName().Name),
                });
            }


        }

        private static PluginsInfo LoadDomains(this IServiceCollection services, IConfiguration configuration)
        {

            var pluginsPath = new PluginsInfo()
            {
                Plugins = configuration.GetSection("Plugins").Get<List<Plugin>>()
            };


            var PluginsInfo = GerenciadorDePlugin.InformePlugins(pluginsPath);


            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {

                foreach (var pluginInfo in PluginsInfo.Plugins)
                {
                    if (args.Name.Contains(pluginInfo.ProjetoDominio))
                        return pluginInfo.LoadAssembly(pluginInfo.ProjetoDominioPath);

                    if (args.Name.Contains(pluginInfo.ProjetoInfraData))
                        return pluginInfo.LoadAssembly(pluginInfo.ProjetoInfraDataPath);

                    if (args.Name.Contains(pluginInfo.ProjetoUIWeb))
                        return pluginInfo.LoadAssembly(pluginInfo.ProjetoUIWebPath);


                }

                throw new Exception("Assembly de dominio não encontrado");
            };




            services.AddSingleton(PluginsInfo);

            return PluginsInfo;
        }
    }

}
