using System.IO;
using System.Reflection;
using System;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System.Linq;

internal class UMBITEmbeddedFileProviderStatic : IFileProvider
{
    private static readonly char[] _caracteresInvalidos = Path.GetInvalidFileNameChars()
           .Where(c => c != '/' && c != '\\').ToArray();

    private readonly string Plugin;
    private readonly Assembly PluginAssembly;
    private readonly string BaseNamespace;
    private readonly DateTimeOffset UltimaModificacao;

    public UMBITEmbeddedFileProviderStatic(Assembly assembly, string plugin) : this(assembly, assembly?.GetName()?.Name, plugin)
    {

    }

    public UMBITEmbeddedFileProviderStatic(Assembly assembly, string baseNamespace, string plugin)
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
        throw new System.NotImplementedException();
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        throw new System.NotImplementedException();
    }

    public IChangeToken Watch(string filter)
    {
        throw new System.NotImplementedException();
    }
}