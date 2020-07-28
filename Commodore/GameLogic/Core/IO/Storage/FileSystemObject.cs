using System;
using System.Collections.Generic;
using System.IO;

namespace Commodore.GameLogic.Core.IO.Storage
{
    [Serializable]
    public class FileSystemObject
    {
        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                if (value.Length > SystemConstants.MaxFileSystemObjectNameLength)
                    throw new PathTooLongException($"the name '{value}' is too long.");

                _name = value;
            }
        }

        public Directory Parent { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime LastModified { get; set; }

        public FileSystemObject()
        {
            DateCreated = DateTime.Now;
            LastModified = DateTime.Now;
        }

        public string GetAbsolutePath()
        {
            var segments = new Stack<string>();
            var current = this;

            while (current.Parent != null)
            {
                segments.Push(current.Name);
                current = current.Parent;
            }

            return "/" + string.Join("/", segments);
        }

        public bool IsParentOf(FileSystemObject fsObject)
        {
            var currentParent = fsObject.Parent;

            while (currentParent != null)
            {
                if (currentParent == this)
                    return true;

                currentParent = currentParent.Parent;
            }

            return false;
        }

        public void RemoveSelf()
        {
            if (Parent != null)
                Parent.Children.Remove(Name);
        }
    }
}
