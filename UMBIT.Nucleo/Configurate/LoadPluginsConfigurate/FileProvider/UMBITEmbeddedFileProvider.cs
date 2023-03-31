using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Embedded;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UMBIT.Nucleo.Configurate.FileProvider
{
    public class UMBITEmbeddedFileProvider : IFileProvider
    {
        private static readonly char[] _caracteresInvalidos = Path.GetInvalidFileNameChars()
           .Where(c => c != '/' && c != '\\').ToArray();

        private readonly string Plugin;
        private readonly Assembly PluginAssembly;
        private readonly string BaseNamespace;
        private readonly DateTimeOffset UltimaModificacao;

        public UMBITEmbeddedFileProvider(Assembly assembly, string plugin) : this(assembly, assembly?.GetName()?.Name, plugin)
        {

        }

        public UMBITEmbeddedFileProvider(Assembly assembly, string baseNamespace, string plugin)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }

            Plugin = plugin;
            BaseNamespace = string.IsNullOrEmpty(baseNamespace) ? string.Empty : baseNamespace + ".";
            PluginAssembly = assembly;

            UltimaModificacao = DateTimeOffset.UtcNow;

            if (!string.IsNullOrEmpty(PluginAssembly.Location))
            {
                try
                {
                    UltimaModificacao = File.GetLastWriteTimeUtc(PluginAssembly.Location);
                }
                catch (PathTooLongException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
        }
        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            if (subpath == null)
            {
                return NotFoundDirectoryContents.Singleton;
            }

            if (subpath.Length != 0 && !string.Equals(subpath, "/", StringComparison.Ordinal))
            {
                return NotFoundDirectoryContents.Singleton;
            }

            var entries = new List<IFileInfo>();

            var resources = PluginAssembly.GetManifestResourceNames();
            for (var i = 0; i < resources.Length; i++)
            {
                var resourceName = resources[i];
                if (resourceName.StartsWith(BaseNamespace, StringComparison.Ordinal))
                {
                    entries.Add(new EmbeddedResourceFileInfo(
                        PluginAssembly,
                        resourceName,
                        resourceName.Substring(BaseNamespace.Length),
                        UltimaModificacao));
                }
            }

            return new UMBITEnumerableDirectoryContents(entries);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            if (string.IsNullOrEmpty(subpath))
            {
                return new NotFoundFileInfo(subpath);
            }

            var caminhoArquivo = subpath;

            if (caminhoArquivo.StartsWith($"/{Plugin}/", StringComparison.Ordinal))
            {
                caminhoArquivo = caminhoArquivo.Substring(Plugin.Length + 2);
            }
            else if (caminhoArquivo.StartsWith($"{Plugin}/", StringComparison.Ordinal))
            {
                caminhoArquivo = caminhoArquivo.Substring(Plugin.Length + 1);
            }
            else
            {
                return new NotFoundFileInfo(caminhoArquivo);
            }

            var builder = new StringBuilder(BaseNamespace.Length + subpath.Length);
            builder.Append(BaseNamespace);
            builder.Append(caminhoArquivo);

            for (var i = BaseNamespace.Length; i < builder.Length; i++)
            {
                if (builder[i] == '/' || builder[i] == '\\')
                {
                    builder[i] = '.';
                }
            }

            var caminhoRecurso = builder.ToString();
            if (PossuiCaracteresInvalidos(caminhoRecurso))
            {
                return new NotFoundFileInfo(caminhoRecurso);
            }

            var nomeArquivo = Path.GetFileName(subpath);
            if (PluginAssembly.GetManifestResourceInfo(caminhoRecurso) == null)
            {
                return new NotFoundFileInfo(nomeArquivo);
            }

            return new EmbeddedResourceFileInfo(PluginAssembly, caminhoRecurso, nomeArquivo, UltimaModificacao);
        }

        public IChangeToken Watch(string filter)
        {
            return NullChangeToken.Singleton;
        }

        private static bool PossuiCaracteresInvalidos(string path)
        {
            return path.IndexOfAny(_caracteresInvalidos) != -1;
        }
    }
}