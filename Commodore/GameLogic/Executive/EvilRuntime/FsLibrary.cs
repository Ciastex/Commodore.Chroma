using Commodore.EVIL;
using Commodore.EVIL.Abstraction;
using Commodore.EVIL.Exceptions;
using Commodore.EVIL.Execution;
using Commodore.EVIL.RuntimeLibrary.Base;
using Commodore.GameLogic.Core;
using Commodore.GameLogic.Core.IO.Storage;
using System.Linq;

namespace Commodore.GameLogic.Executive.EvilRuntime
{
    public class FsLibrary : ClrPackage
    {
        public DynValue Files(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectAtLeast(1)
                .ExpectAtMost(2)
                .ExpectTypeAtIndex(0, DynValueType.String);

            var abs = false;
            if (args.Count == 2)
            {
                args.ExpectTypeAtIndex(1, DynValueType.Number);
                abs = args[1].Number != 0;
            }

            var path = args[0].String;

            try
            {
                var table = new Table();

                var files = Directory.GetFiles(path, abs);

                for (var i = 0; i < files.Count; i++)
                    table[i] = new DynValue(files[i]);

                return new DynValue(table);
            }
            catch
            {
                throw new ClrFunctionException($"Couldn't retrieve file list for path '{path}'.");
            }
        }

        public DynValue Directories(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectAtLeast(1)
                .ExpectAtMost(2)
                .ExpectTypeAtIndex(0, DynValueType.String);

            var abs = false;
            if (args.Count == 2)
            {
                args.ExpectTypeAtIndex(1, DynValueType.Number);
                abs = args[1].Number != 0;
            }

            var path = args[0].String;

            try
            {
                var table = new Table();

                var dirs = Directory.GetDirectories(path, abs);

                for (var i = 0; i < dirs.Count; i++)
                    table[i] = new DynValue(dirs[i]);

                return new DynValue(table);
            }
            catch
            {
                throw new ClrFunctionException($"Couldn't retrieve directory list for path '{path}'.");
            }
        }

        public DynValue CurrentWorkingDirectory(Interpreter interpreter, ClrFunctionArguments args)
        {
            return new DynValue(Kernel.Instance.FileSystemContext.WorkingDirectory.GetAbsolutePath());
        }

        public DynValue ChangeDirectory(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(1)
                .ExpectTypeAtIndex(0, DynValueType.String);

            var path = args[0].String;

            try
            {
                Directory.ChangeWorkingDirectory(path);
                return DynValue.Zero;
            }
            catch
            {
                return new DynValue(-1);
            }
        }

        public DynValue Remove(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectAtLeast(1)
                .ExpectTypeAtIndex(0, DynValueType.String);

            var forceRemoveDirectory = false;

            if (args.Count == 2)
            {
                args.ExpectTypeAtIndex(1, DynValueType.Number);

                if (args[1].Number != 0)
                    forceRemoveDirectory = true;
            }

            var path = args[0].String;
            if (Directory.Exists(path))
            {
                var dir = Directory.GetDirectory(path);
                if (dir.Children.Count > 0)
                {
                    if (!forceRemoveDirectory)
                        return new DynValue(-1);
                }

                Directory.RemoveDirectory(path);
                return DynValue.Zero;
            }
            else if (File.Exists(path))
            {
                File.Remove(path);
                return DynValue.Zero;
            }

            return new DynValue(-2);
        }

        public DynValue Lines(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(1)
                .ExpectTypeAtIndex(0, DynValueType.String);

            var path = args[0].String;

            if (!File.Exists(path))
                return new DynValue(-1);

            var lines = File.Get(path).GetData().Split('\n');

            var table = new Table();
            for (var i = 0; i < lines.Length; i++)
                table[i] = new DynValue(lines[i]);

            return new DynValue(table);
        }

        public DynValue Copy(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(2)
                .ExpectTypeAtIndex(0, DynValueType.String)
                .ExpectTypeAtIndex(1, DynValueType.String);

            var source = args[0].String;
            var target = args[1].String;

            try
            {
                File.Copy(source, target);
                return DynValue.Zero;
            }
            catch
            {
                return new DynValue(-1);
            }
        }

        public DynValue Touch(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(1)
                .ExpectTypeAtIndex(0, DynValueType.String);

            var target = args[0].String;

            try
            {
                File.Create(target);
                return DynValue.Zero;
            }
            catch
            {
                return new DynValue(-1);
            }
        }

        public DynValue MakeDir(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(1)
                .ExpectTypeAtIndex(0, DynValueType.String);

            var target = args[0].String;

            try
            {
                Directory.CreateDirectory(target);
                return DynValue.Zero;
            }
            catch
            {
                return new DynValue(-1);
            }
        }

        public DynValue Move(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(2)
                .ExpectTypeAtIndex(0, DynValueType.String)
                .ExpectTypeAtIndex(1, DynValueType.String);

            var source = args[0].String;
            var target = args[1].String;

            try
            {
                if (Directory.Exists(source))
                {
                    Directory.Move(source, target);
                }
                else if (File.Exists(source))
                {
                    File.Move(source, target);
                }
                else
                {
                    return new DynValue(-2);
                }

                return DynValue.Zero;
            }
            catch
            {
                return new DynValue(-1);
            }
        }

        public DynValue Exists(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(1)
                .ExpectTypeAtIndex(0, DynValueType.String);

            var path = args[0].String;

            if (File.Exists(path))
                return new DynValue(1);
            else if (Directory.Exists(path))
                return new DynValue(2);
            else
                return DynValue.Zero;
        }

        public DynValue SetAttributes(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(2)
                .ExpectTypeAtIndex(0, DynValueType.String)
                .ExpectTypeAtIndex(1, DynValueType.String);

            var path = args[0].String;
            var attrs = args[1].String.Distinct();

            if (!File.Exists(path))
                return new DynValue(-1);

            FileAttributes fileAttrs = 0;
            foreach (var c in attrs)
            {
                if (!File.SupportedAttributeDescriptors.Contains(c) && !(c == '-'))
                    return new DynValue(-2);

                if (c == 'x')
                {
                    fileAttrs |= FileAttributes.Executable;
                    continue;
                }
                else if (c == '-')
                {
                    fileAttrs &= ~(FileAttributes.Executable);
                    continue;
                }

                if (c == 'h')
                {
                    fileAttrs |= FileAttributes.Hidden;
                }
                else if (c == '-')
                {
                    fileAttrs &= ~(FileAttributes.Hidden);
                }
            }

            var file = File.Get(path);
            file.Attributes = fileAttrs;

            return DynValue.Zero;
        }

        public DynValue GetAttributes(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(1)
                .ExpectTypeAtIndex(0, DynValueType.String);

            var path = args[0].String;

            if (!File.Exists(path))
                return new DynValue(-1);

            var attrs = File.Get(path).Attributes;
            var ret = string.Empty;

            if ((attrs & FileAttributes.Executable) != 0)
                ret += "x";
            else
                ret += "-";

            if ((attrs & FileAttributes.Hidden) != 0)
                ret += "h";
            else
                ret += "-";

            return new DynValue(ret);
        }

        public DynValue Write(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(2)
                .ExpectTypeAtIndex(0, DynValueType.String)
                .ExpectTypeAtIndex(1, DynValueType.String);

            var path = args[0].String;
            var data = args[1].String;

            try
            {
                File.Create(path, true).SetData(data);
                return DynValue.Zero;
            }
            catch
            {
                return new DynValue(-1);
            }
        }

        public DynValue WriteAllLines(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(2)
                .ExpectTypeAtIndex(0, DynValueType.String)
                .ExpectTypeAtIndex(1, DynValueType.Table);

            var path = args[0].String;
            var table = args[1].Table;

            try
            {
                File.Create(path, true).SetData(
                    string.Join("\n", table.Values.Select(x => x.AsString().String))
                );

                return DynValue.Zero;
            }
            catch
            {
                return new DynValue(-1);
            }
        }

        public override void Register(Environment env, Interpreter interpreter)
        {
            env.RegisterBuiltIn("fs.files", Files);
            env.RegisterBuiltIn("fs.dirs", Directories);
            env.RegisterBuiltIn("fs.cwd", CurrentWorkingDirectory);
            env.RegisterBuiltIn("fs.cd", ChangeDirectory);
            env.RegisterBuiltIn("fs.rm", Remove);
            env.RegisterBuiltIn("fs.lines", Lines);
            env.RegisterBuiltIn("fs.cp", Copy);
            env.RegisterBuiltIn("fs.md", MakeDir);
            env.RegisterBuiltIn("fs.touch", Touch);
            env.RegisterBuiltIn("fs.mv", Move);
            env.RegisterBuiltIn("fs.write", Write);
            env.RegisterBuiltIn("fs.writeall", WriteAllLines);
            env.RegisterBuiltIn("fs.exists", Exists);
            env.RegisterBuiltIn("fs.setattr", SetAttributes);
            env.RegisterBuiltIn("fs.getattr", GetAttributes);
        }
    }
}