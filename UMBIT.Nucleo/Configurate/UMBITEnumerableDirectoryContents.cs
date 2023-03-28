using Microsoft.Extensions.FileProviders;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UMBIT.Nucleo.Configurate
{
    internal class UMBITEnumerableDirectoryContents : IDirectoryContents
    {
        private readonly IEnumerable<IFileInfo> _entries;

        public UMBITEnumerableDirectoryContents(IEnumerable<IFileInfo> entries)
        {
            if (entries == null)
            {
                throw new ArgumentNullException(nameof(entries));
            }

            _entries = entries;
        }

        public bool Exists
        {
            get { return true; }
        }

        public IEnumerator<IFileInfo> GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _entries.GetEnumerator();
        }
    }
}