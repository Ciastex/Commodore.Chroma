using Commodore.GameLogic.Core.IO.Storage.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Commodore.GameLogic.Core.IO.Storage
{
    [Serializable]
    public class Directory : FileSystemObject
    {
        public bool IsRoot => Parent == null;

        public Dictionary<string, FileSystemObject> Children { get; }

        public Directory()
        {
            Name = string.Empty;
            Children = new Dictionary<string, FileSystemObject>();
        }

        public Directory(string name, Directory parent) : this()
        {
            Name = name;
            Parent = parent;
        }
        
        public Directory Subdirectory(string name)
            => Children[name] as Directory;

        public File File(string name)
            => Children[name] as File;

        public File AddNewFile(string name, FileAttributes attributes = 0)
        {
            AddFile(name, new File(name, this) { Attributes = attributes });
            return Children[name] as File;
        }

        public void AppendChildren(Dictionary<string, FileSystemObject> children)
        {
            foreach (var c in children)
                Children.Add(c.Key, c.Value);
        }

        public void RemoveExistingDirectory(string name)
        {
            if (Children.ContainsKey(name) && Children[name] is Directory)
            {
                Children.Remove(name);
            }
            else
            {
                throw new DirectoryNotFoundException(name, $"The provided directory was not found in '{GetAbsolutePath()}'");
            }
        }

        public void RemoveExistingFile(string name)
        {
            if (Children.ContainsKey(name) && Children[name] is File)
            {
                Children.Remove(name);
            }
            else
            {
                throw new FileNotFoundException(name, $"The provided file was not found in '{GetAbsolutePath()}'");
            }
        }

        public Directory CreateGivenPath(string path)
        {
            if (Path.ContainsInvalidCharacters(path))
                throw new InvalidPathException(path, "The provided path contains invalid characters.");

            var current = this;

            // we don't support empty strings
            if (string.IsNullOrEmpty(path))
                throw new InvalidOperationException("The provided path was empty.");

            // we don't support special paths here either
            if (path == "/" || path == SpecialFileNames.CurrentWorkingDirectory || path == SpecialFileNames.ParentDirectory)
                throw new InvalidPathException(path, "The provided path was invalid.");

            var segments = path.Split('/').Where(x => !string.IsNullOrEmpty(x)).ToArray();

            if (segments.Length == 0)
                throw new InvalidOperationException("Already exists.");

            for (var i = 0; i < segments.Length; i++)
            {
                // this is our creation target
                if (i + 1 >= segments.Length)
                {
                    var targetName = segments[i];

                    if (targetName == "/" || targetName == SpecialFileNames.CurrentWorkingDirectory || targetName == SpecialFileNames.ParentDirectory)
                        throw new InvalidOperationException("The directory name is invalid.");

                    if (current.HasDirectoryChildNamed(targetName))
                        throw new InvalidOperationException($"The directory '{targetName}' already exists in the destination directory.");

                    return current.AddNewDirectory(targetName);
                }
                else // gotta traverse some more
                {
                    var segment = segments[i];

                    // ignore things like '/////'
                    if (string.IsNullOrEmpty(segment))
                        continue;

                    // skip references to current directory
                    if (segment == SpecialFileNames.CurrentWorkingDirectory)
                        continue;

                    if (segment == SpecialFileNames.ParentDirectory)
                    {
                        if (current.IsRoot)
                            throw new DirectoryNotFoundException(segment, "Cannot go above the root directory.");

                        current = current.Parent;
                        continue;
                    }

                    if (!current.HasDirectoryChildNamed(segment))
                        current.AddNewDirectory(segment);

                    current = current.Children[segment] as Directory;
                }
            }

            return current;
        }

        public static bool Exists(string path, bool forceLocalContext = false)
        {
            if (Path.ContainsInvalidCharacters(path))
                throw new InvalidPathException(path, "The provided path contains invalid characters.");

            Directory startingPoint;
            Directory current;

            // we don't support empty strings
            if (string.IsNullOrEmpty(path))
                return false;

            // assume root and this directory always exist
            if (path == "/" || path == SpecialFileNames.CurrentWorkingDirectory)
                return true;

            if (Path.IsAbsolute(path))
                startingPoint = forceLocalContext ? Kernel.Instance.LocalSystemContext.RootDirectory :
                                                    Kernel.Instance.CurrentSystemContext.RootDirectory;
            else
                startingPoint = forceLocalContext ? Kernel.Instance.LocalSystemContext.WorkingDirectory :
                                                    Kernel.Instance.CurrentSystemContext.WorkingDirectory;

            current = startingPoint;
            var segments = path.Split('/');

            for (var i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];

                // skip things like '/////'
                if (string.IsNullOrEmpty(segment))
                    continue;

                if (segment == SpecialFileNames.CurrentWorkingDirectory)
                    continue;

                if (segment == SpecialFileNames.ParentDirectory)
                {
                    if (current.IsRoot)
                        return false;

                    current = current.Parent as Directory;
                    continue;
                }

                if (current.HasDirectoryChildNamed(segment))
                {
                    current = current.Children[segment] as Directory;
                    continue;
                }
                else
                {
                    // path broken in progress of traversal
                    return false;
                }
            }

            // if we arrived here it means that we reached the target
            return true;
        }

        public static void CreateDirectory(string path, bool forceLocalContext = false)
        {
            if (Path.ContainsInvalidCharacters(path))
                throw new InvalidPathException(path, "The provided path contains invalid characters.");

            Directory startingPoint;
            Directory current;

            // we don't support empty strings
            if (string.IsNullOrEmpty(path))
                throw new InvalidOperationException("The provided path was empty.");

            // we don't support special paths here either
            if (path == "/" || path == SpecialFileNames.CurrentWorkingDirectory || path == SpecialFileNames.ParentDirectory)
                throw new InvalidPathException(path, "The provided path was invalid.");

            if (Path.IsAbsolute(path))
                startingPoint = forceLocalContext ? Kernel.Instance.LocalSystemContext.RootDirectory :
                                                    Kernel.Instance.CurrentSystemContext.RootDirectory;
            else
                startingPoint = forceLocalContext ? Kernel.Instance.LocalSystemContext.WorkingDirectory :
                                                    Kernel.Instance.CurrentSystemContext.WorkingDirectory;

            current = startingPoint;
            var segments = path.Split('/').Where(x => !string.IsNullOrEmpty(x)).ToArray();

            if (segments.Length == 0)
                throw new InvalidOperationException("Already exists.");

            for (var i = 0; i < segments.Length; i++)
            {
                // this is our creation target
                if (i + 1 >= segments.Length)
                {
                    var targetName = segments[i];

                    if (targetName == "/" || targetName == SpecialFileNames.CurrentWorkingDirectory || targetName == SpecialFileNames.ParentDirectory)
                        throw new InvalidOperationException("The directory name is invalid.");

                    if (current.HasDirectoryChildNamed(targetName))
                        throw new InvalidOperationException($"The directory '{targetName}' already exists in the destination directory.");

                    current.AddNewDirectory(targetName);
                    return;
                }
                else // gotta traverse some more
                {
                    var segment = segments[i];

                    // ignore things like '/////'
                    if (string.IsNullOrEmpty(segment))
                        continue;

                    // skip references to current directory
                    if (segment == SpecialFileNames.CurrentWorkingDirectory)
                        continue;

                    if (segment == SpecialFileNames.ParentDirectory)
                    {
                        if (current.IsRoot)
                            throw new DirectoryNotFoundException(segment, "Cannot go above the root directory.");

                        current = current.Parent;
                        continue;
                    }

                    if (current.HasDirectoryChildNamed(segment))
                    {
                        current = current.Children[segment] as Directory;
                        continue;
                    }
                    else
                    {
                        throw new DirectoryNotFoundException(segment, $"The directory was not found in the provided path '{path}'.");
                    }
                }
            }
        }

        public static void RemoveDirectory(string path, bool forceLocalContext = false)
        {
            if (Path.ContainsInvalidCharacters(path))
                throw new InvalidPathException(path, "The provided path contains invalid characters.");

            Directory startingPoint;
            Directory current;

            // we don't support empty strings
            if (string.IsNullOrEmpty(path))
                throw new InvalidOperationException("The provided path was empty.");

            // we don't support special paths here either
            if (path == "/" || path == SpecialFileNames.CurrentWorkingDirectory || path == SpecialFileNames.ParentDirectory)
                throw new InvalidPathException(path, "The provided path was invalid.");

            if (Path.IsAbsolute(path))
                startingPoint = forceLocalContext ? Kernel.Instance.LocalSystemContext.RootDirectory :
                                                    Kernel.Instance.CurrentSystemContext.RootDirectory;
            else
                startingPoint = forceLocalContext ? Kernel.Instance.LocalSystemContext.WorkingDirectory :
                                                    Kernel.Instance.CurrentSystemContext.WorkingDirectory;

            current = startingPoint;
            var segments = path.Split('/').Where(x => !string.IsNullOrEmpty(x)).ToArray();

            if (segments.Length == 0)
                throw new InvalidOperationException("Cannot remove root directory.");

            for (var i = 0; i < segments.Length; i++)
            {
                // this is our removal target
                if (i + 1 >= segments.Length)
                {
                    var targetName = segments[i];

                    if (!current.HasDirectoryChildNamed(targetName))
                        throw new InvalidOperationException($"The directory '{targetName}' does not exist in the destination directory.");

                    current.RemoveExistingDirectory(targetName);
                    return;
                }
                else // gotta traverse some more
                {
                    var segment = segments[i];

                    // ignore things like '/////'
                    if (string.IsNullOrEmpty(segment))
                        continue;

                    // skip references to current directory
                    if (segment == SpecialFileNames.CurrentWorkingDirectory)
                        continue;

                    if (segment == SpecialFileNames.ParentDirectory)
                    {
                        if (current.IsRoot)
                            throw new DirectoryNotFoundException(segment, "Cannot go above the root directory.");

                        current = current.Parent;
                        continue;
                    }

                    if (current.HasDirectoryChildNamed(segment))
                    {
                        current = current.Children[segment] as Directory;
                        continue;
                    }
                    else
                    {
                        throw new DirectoryNotFoundException(segment, $"An intermediate directory was not found in the provided path '{path}'.");
                    }
                }
            }
        }

        public static void ChangeWorkingDirectory(string path, bool forceLocalContext = false)
        {
            if (Path.ContainsInvalidCharacters(path))
                throw new InvalidPathException(path, "The provided path contains invalid characters.");

            Directory startingPoint;
            Directory current;

            // we don't support empty strings
            if (string.IsNullOrEmpty(path))
                throw new InvalidOperationException("The provided path was empty.");

            if (path == SpecialFileNames.CurrentWorkingDirectory)
                return;

            // handle root path directly
            if (path == "/")
            {
                Kernel.Instance.CurrentSystemContext.WorkingDirectory = Kernel.Instance.CurrentSystemContext.RootDirectory;
                return;
            }

            if (Path.IsAbsolute(path))
                startingPoint = forceLocalContext ? Kernel.Instance.LocalSystemContext.RootDirectory :
                                                    Kernel.Instance.CurrentSystemContext.RootDirectory;
            else
                startingPoint = forceLocalContext ? Kernel.Instance.LocalSystemContext.WorkingDirectory :
                                                    Kernel.Instance.CurrentSystemContext.WorkingDirectory;

            current = startingPoint;
            var segments = path.Split('/').Where(x => !string.IsNullOrEmpty(x)).ToArray();

            for (var i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];

                // skip things like '/////'
                if (string.IsNullOrEmpty(segment))
                    continue;

                if (segment == SpecialFileNames.CurrentWorkingDirectory)
                    continue;

                if (segment == SpecialFileNames.ParentDirectory)
                {
                    if (current.IsRoot)
                        continue; // don't throw here, just move along ignoring the overshoot

                    current = current.Parent as Directory;
                    continue;
                }

                if (current.HasDirectoryChildNamed(segment))
                {
                    current = current.Children[segment] as Directory;
                    continue;
                }
                else
                {
                    throw new DirectoryNotFoundException(segment, $"An intermediate directory was not found in the provided path '{path}'.");
                }
            }

            // if we arrived here safely, change directory
            Kernel.Instance.CurrentSystemContext.WorkingDirectory = current;
        }

        public static Directory GetDirectory(string path, bool forceLocalContext = false)
        {
            if (Path.ContainsInvalidCharacters(path))
                throw new InvalidPathException(path, "The provided path contains invalid characters.");

            Directory startingPoint;
            Directory current;

            if (string.IsNullOrEmpty(path))
                throw new InvalidOperationException("The provided path was empty.");

            if (path == SpecialFileNames.CurrentWorkingDirectory)
                return Kernel.Instance.CurrentSystemContext.WorkingDirectory;

            // handle root path directly
            if (path == "/")
                return Kernel.Instance.CurrentSystemContext.RootDirectory;

            if (Path.IsAbsolute(path))
                startingPoint = forceLocalContext ? Kernel.Instance.LocalSystemContext.RootDirectory :
                                                    Kernel.Instance.CurrentSystemContext.RootDirectory;
            else
                startingPoint = forceLocalContext ? Kernel.Instance.LocalSystemContext.WorkingDirectory :
                                                    Kernel.Instance.CurrentSystemContext.WorkingDirectory;

            current = startingPoint;
            var segments = path.Split('/').Where(x => !string.IsNullOrEmpty(x)).ToArray();

            for (var i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];

                // skip things like '/////'
                if (string.IsNullOrEmpty(segment))
                    continue;

                if (segment == SpecialFileNames.CurrentWorkingDirectory)
                    continue;

                if (segment == SpecialFileNames.ParentDirectory)
                {
                    if (current.IsRoot)
                        throw new DirectoryNotFoundException(segment, "Cannot go above the root directory.");

                    current = current.Parent as Directory;
                    continue;
                }

                if (current.HasDirectoryChildNamed(segment))
                {
                    current = current.Children[segment] as Directory;
                    continue;
                }
                else
                {
                    throw new DirectoryNotFoundException(segment, $"Directory at the provided path does not exist '{path}'.");
                }
            }

            return current;
        }

        public static void Move(string from, string to)
        {
            var dirFrom = GetDirectory(from);

            if (!Exists(to))
            {
                CreateDirectory(to);
                var dirTo = GetDirectory(to);

                if (dirFrom == dirTo)
                    throw new InvalidOperationException("The destination directory is the same as source directory.");

                if (dirFrom.IsParentOf(dirTo))
                    throw new InvalidOperationException("The destination directory is a child of the source directory.");

                dirTo.AppendChildren(dirFrom.Children);
                RemoveDirectory(from);
            }
            else
            {
                var dirTo = GetDirectory(to);

                if (dirFrom == dirTo)
                    throw new InvalidOperationException("The destination directory is the same as source directory.");

                if (dirFrom.IsParentOf(dirTo))
                    throw new InvalidOperationException("The destination directory is a child of the source directory.");

                if (dirTo.HasDirectoryChildNamed(dirFrom.Name))
                    throw new InvalidOperationException("The destination directory already exists.");

                dirFrom.Parent.Children.Remove(dirFrom.Name);

                dirFrom.Parent = dirTo;
                dirTo.Children.Add(dirFrom.Name, dirFrom);
            }
        }

        public static List<string> GetFiles(string path, bool absolutePaths = false, bool forceLocalContext = false)
        {
            var directory = GetDirectory(path, forceLocalContext);
            var files = directory.Children.Values.Where(x => x is File);

            if (absolutePaths)
                return files.OrderBy(x => x.Name).Select(x => x.GetAbsolutePath()).ToList();
            else
                return files.Select(x => x.Name).OrderBy(x => x).ToList();
        }

        public static List<string> GetDirectories(string path, bool absolutePaths = false, bool forceLocalContext = false)
        {
            var directory = GetDirectory(path, forceLocalContext);
            var directories = directory.Children.Values.Where(x => x is Directory);

            if (absolutePaths)
                return directories.OrderBy(x => x.Name).Select(x => x.GetAbsolutePath()).ToList();
            else
                return directories.Select(x => x.Name).OrderBy(x => x).ToList();
        }

        public bool HasDirectoryChildNamed(string name)
            => Children.ContainsKey(name) && Children[name] is Directory;

        public bool HasFileChildNamed(string name)
            => Children.ContainsKey(name) && Children[name] is File;

        public Directory AddNewDirectory(string name)
        {
            if (!Children.ContainsKey(name))
            {
                var directory = new Directory(name, this);
                Children.Add(name, directory);

                return directory;
            }
            else
            {
                var existingThing = "directory";

                if (Children[name] is File)
                    existingThing = "file";

                throw new InvalidOperationException($"Directory '{GetAbsolutePath()}' already contains a {existingThing} called '{name}'.");
            }
        }

        private void AddFile(string name, File file)
        {
            if (name == SpecialFileNames.CurrentWorkingDirectory || name == SpecialFileNames.ParentDirectory)
                throw new InvalidOperationException($"Cannot create file with a reserved name '{name}'.");

            if (!Children.ContainsKey(name))
            {
                Children.Add(name, file);
            }
            else
            {
                var existingThing = "directory";

                if (Children[name] is File)
                    existingThing = "file";

                throw new InvalidOperationException($"Directory '{GetAbsolutePath()}' already contains a {existingThing} called '{name}'.");
            }
        }
    }
}
