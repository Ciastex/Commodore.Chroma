using Commodore.GameLogic.Core.IO.Storage.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Commodore.GameLogic.Core.IO.Storage
{
    [Serializable]
    public class File : FileSystemObject
    {
        public static readonly List<char> SupportedAttributeDescriptors = new List<char>()
        {
            'x',
            'h'
        };

        public FileAttributes Attributes { get; set; }
        public byte[] Data { get; set; }

        public File(string name, Directory parent)
        {
            Name = name;

            Parent = parent;
            Data = new byte[0];
        }

        public File SetData(string data)
            => SetData(Encoding.UTF8.GetBytes(data));

        public File SetData(byte[] data)
        {
            Data = data;
            LastModified = DateTime.Now;

            return this;
        }

        public string GetData()
        {
            return Encoding.UTF8.GetString(Data);
        }

        public static bool Exists(string path, bool forceLocalContext = false)
        {
            if (Path.ContainsInvalidCharacters(path))
                throw new InvalidPathException(path, "The path contains invalid characters.");

            try
            {
                var containingDirectoryPath = Path.GetDirectoryName(path);
                var containingDirectory = Directory.GetDirectory(containingDirectoryPath, forceLocalContext);

                var fileName = Path.GetFileName(path);

                return containingDirectory.HasFileChildNamed(fileName);
            }
            catch (DirectoryNotFoundException)
            {
                return false;
            }
        }

        public static File Create(string path, bool overwrite = false, bool forceLocalContext = false, Directory forceStartingPoint = null)
        {
            if (Path.ContainsInvalidCharacters(path))
                throw new InvalidPathException(path, "The path contains invalid characters.");

            FileAttributes previousAttributes = 0;
            if (Exists(path))
            {
                if (overwrite)
                {
                    previousAttributes = Get(path).Attributes;
                    Remove(path);
                }
                else
                    throw new InvalidOperationException($"The file '{path}' already exists.");
            }

            var containingDirectoryPath = Path.GetDirectoryName(path);
            var containingDirectory = Directory.GetDirectory(containingDirectoryPath, forceLocalContext);

            var fileName = Path.GetFileName(path);

            var file = containingDirectory.AddNewFile(fileName);
            file.Attributes = previousAttributes;

            return file;
        }

        public static File Get(string path, bool forceLocalContext = false)
        {
            if (Path.ContainsInvalidCharacters(path))
                throw new InvalidPathException(path, "The path contains invalid characters.");

            if (!Exists(path))
                throw new FileNotFoundException(path, $"File '{path}' does not exist.");

            var containingDirectoryPath = Path.GetDirectoryName(path);
            var containingDirectory = Directory.GetDirectory(containingDirectoryPath, forceLocalContext);

            var fileName = Path.GetFileName(path);
            return containingDirectory.Children[fileName] as File;
        }

        public static void Remove(string path, bool forceLocalContext = false)
        {
            if (!Exists(path))
                throw new FileNotFoundException(path, $"Cannot remove '{path}'- the file was not found.");

            var fileName = Path.GetFileName(path);
            var containingDirectoryPath = Path.GetDirectoryName(path);
            var containingDirectory = Directory.GetDirectory(containingDirectoryPath, forceLocalContext);

            containingDirectory.RemoveExistingFile(fileName);
        }

        public static File Copy(string sourcePath, string targetPath, bool forceLocalContext = false)
        {
            if (Path.ContainsInvalidCharacters(sourcePath))
                throw new InvalidPathException(sourcePath, "The source path contains invalid characters.");

            if (Path.ContainsInvalidCharacters(targetPath))
                throw new InvalidPathException(targetPath, "The target path contains invalid characters.");

            if (!Exists(sourcePath))
                throw new FileNotFoundException(sourcePath, $"The source path '{sourcePath}' does not exist.");

            if (Exists(targetPath))
                throw new InvalidOperationException($"The copy target file '{targetPath}' already exists.");

            if (Directory.Exists(targetPath))
            {
                var dir = Directory.GetDirectory(targetPath, forceLocalContext);

                var targetFileName = Path.GetFileName(sourcePath);

                var sourceFile = Get(sourcePath);
                var targetFile = dir.AddNewFile(targetFileName);

                targetFile.SetData(sourceFile.GetData());
                targetFile.Attributes = sourceFile.Attributes;
                return targetFile;
            }
            else
            {
                var sourceFile = Get(sourcePath, forceLocalContext);
                var targetFile = Create(targetPath, false, forceLocalContext);

                targetFile.SetData(sourceFile.GetData());
                targetFile.Attributes = sourceFile.Attributes;
                
                return targetFile;
            }
        }

        public static File Move(string sourcePath, string targetPath, bool forceLocalContext = false)
        {
            var ret = Copy(sourcePath, targetPath, forceLocalContext);
            Remove(sourcePath);

            return ret;
        }
    }
}
