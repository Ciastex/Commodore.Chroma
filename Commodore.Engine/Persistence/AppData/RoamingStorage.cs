using System;
using System.IO;

namespace Commodore.Engine.Persistence.AppData
{
    public static class RoamingStorage
    {
        public static void CreateDirectoryIfDoesNotExist(string directory)
        {
            var target = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), directory);

            if (!Directory.Exists(target))
                Directory.CreateDirectory(target);
        }
        public static Stream OpenRead(string path)
        {
            return File.OpenRead(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), path));
        }

        public static Stream OpenWrite(string path)
        {
            return File.OpenWrite(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), path));
        }

        public static bool FileExists(string path)
        {
            return File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), path));
        }

        public static void RemoveFile(string path)
        {
            File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), path));
        }
    }
}
