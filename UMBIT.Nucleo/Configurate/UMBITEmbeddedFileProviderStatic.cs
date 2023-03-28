using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

internal class UMBITEmbeddedFileProviderStatic : IFileProvider
{
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