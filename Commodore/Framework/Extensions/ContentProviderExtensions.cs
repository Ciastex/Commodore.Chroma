using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chroma.ContentManagement;

namespace Commodore.Framework.Extensions
{
    public static class ContentProviderExtensions
    {
        public static List<string> GetContentFileNames(this IContentProvider provider, string relativePath)
        {
            var directoryPath = Path.Combine(provider.ContentRoot, relativePath);
            var files = Directory.GetFiles(directoryPath);
            
            return files.Select(x => Path.GetFileName(x)).ToList();
        }
    }
}