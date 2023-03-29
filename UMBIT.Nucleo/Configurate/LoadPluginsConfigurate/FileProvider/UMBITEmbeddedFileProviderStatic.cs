using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Embedded;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UMBIT.Nucleo.Configurate.FileProvider;

internal class UMBITEmbeddedFileProviderStatic : IFileProvider
{
    private static readonly char[] _caracteresInvalidos = Path.GetInvalidFileNameChars()
           .Where(c => c != '/' && c != '\\').ToArray();

    private readonly Assembly PluginAssembly;
    private readonly string BaseNamespace;
    private readonly DateTimeOffset UltimaModificacao;


    public UMBITEmbeddedFileProviderStatic(Assembly assembly)
    {
        if (assembly == null)
        {
            throw new ArgumentNullException("assembly");
        }

        BaseNamespace = assembly.GetName().Name;
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

        var builder = new StringBuilder(BaseNamespace.Length + subpath.Length);
        builder.Append(BaseNamespace);
        builder.Append(subpath);

        for (var i = BaseNamespace.Length; i < builder.Length; i++)
        {
            if (builder[i] == '/' || builder[i] == '\\')
            {
                builder[i] = '.';
            }
        }

        var caminhoRecurso = BaseNamespace + ".Content." + builder.ToString();
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
        throw new System.NotImplementedException();
    }

    private static bool PossuiCaracteresInvalidos(string path)
    {
        return path.IndexOfAny(_caracteresInvalidos) != -1;
    }
}