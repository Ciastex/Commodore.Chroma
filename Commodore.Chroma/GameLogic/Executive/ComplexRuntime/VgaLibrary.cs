using Commodore.EVIL;
using Commodore.EVIL.Abstraction;
using Commodore.EVIL.Exceptions;
using Commodore.EVIL.Execution;
using Commodore.EVIL.RuntimeLibrary.Base;
using Commodore.GameLogic.Core;
using Commodore.GameLogic.Display;
using Chroma.Graphics;

namespace Commodore.GameLogic.Executive.EVILRuntime
{
    public class VgaLibrary : ClrPackage
    {
        public DynValue GetCursorX(Interpreter interpreter, ClrFunctionArguments args)
            => new DynValue(Kernel.Instance.Vga.CursorX);

        public DynValue GetCursorY(Interpreter interpreter, ClrFunctionArguments args)
            => new DynValue(Kernel.Instance.Vga.CursorY);

        public DynValue Rows(Interpreter interpreter, ClrFunctionArguments args)
            => new DynValue(Kernel.Instance.Vga.TotalRows);

        public DynValue Cols(Interpreter interpreter, ClrFunctionArguments args)
            => new DynValue(Kernel.Instance.Vga.TotalColumns);

        public DynValue SetXY(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(3)
                .ExpectTypeAtIndex(0, DynValueType.String)
                .ExpectIntegerAtIndex(1)
                .ExpectIntegerAtIndex(2);

            var str = args[0].String;
            var x = args[1].Number;
            var y = args[2].Number;

            if (x >= Kernel.Instance.Vga.TotalColumns)
                return new DynValue(-1);

            if (y >= Kernel.Instance.Vga.TotalRows)
                return new DynValue(-1);

            if (str.Length != 1)
                throw new ClrFunctionException("Expected a single-character string.");

            Kernel.Instance.Vga.PutCharAt(str[0], (int)x, (int)y);
            return DynValue.Zero;
        }

        public DynValue SetCursorPosition(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(2)
                .ExpectIntegerAtIndex(0)
                .ExpectIntegerAtIndex(1);

            var x = args[0].Number;
            var y = args[1].Number;

            if (x >= Kernel.Instance.Vga.TotalColumns)
                return new DynValue(-1);

            if (y >= Kernel.Instance.Vga.TotalRows)
                return new DynValue(-1);

            Kernel.Instance.Vga.CursorX = (int)x;
            Kernel.Instance.Vga.CursorY = (int)y;

            return DynValue.Zero;
        }

        public DynValue SetTextModeForeground(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(3)
                .ExpectByteAtIndex(0)
                .ExpectByteAtIndex(1)
                .ExpectByteAtIndex(2);

            var r = (byte)args[0].Number;
            var g = (byte)args[1].Number;
            var b = (byte)args[2].Number;

            Kernel.Instance.Vga.ActiveForegroundColor = new Color(r, g, b);

            return DynValue.Zero;
        }

        public DynValue SetTextModeBackground(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(3)
                .ExpectByteAtIndex(0)
                .ExpectByteAtIndex(1)
                .ExpectByteAtIndex(2);

            var r = (byte)args[0].Number;
            var g = (byte)args[1].Number;
            var b = (byte)args[2].Number;

            Kernel.Instance.Vga.ActiveBackgroundColor = new Color(r, g, b);

            return DynValue.Zero;
        }

        public DynValue ResetTextModeBackground(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectNone();

            Kernel.Instance.Vga.ActiveBackgroundColor = Kernel.Instance.Vga.DefaultBackgroundColor;

            return DynValue.Zero;
        }

        public DynValue ResetTextModeForeground(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectNone();

            Kernel.Instance.Vga.ActiveForegroundColor = Kernel.Instance.Vga.DefaultForegroundColor;

            return DynValue.Zero;
        }

        public DynValue Clear(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectNone();

            Kernel.Instance.Vga.ClearScreen(false);

            return DynValue.Zero;
        }

        public override void Register(Environment env, Interpreter interpreter)
        {
            env.RegisterBuiltIn("vga.cur_x", GetCursorX);
            env.RegisterBuiltIn("vga.cur_y", GetCursorY);
            env.RegisterBuiltIn("vga.setcurpos", SetCursorPosition);
            env.RegisterBuiltIn("vga.setxy", SetXY);
            env.RegisterBuiltIn("vga.rows", Rows);
            env.RegisterBuiltIn("vga.cols", Cols);
            env.RegisterBuiltIn("vga.setfg", SetTextModeForeground);
            env.RegisterBuiltIn("vga.setbg", SetTextModeBackground);
            env.RegisterBuiltIn("vga.resetfg", ResetTextModeForeground);
            env.RegisterBuiltIn("vga.resetbg", ResetTextModeBackground);
            env.RegisterBuiltIn("vga.clear", Clear);
        }
    }
}
