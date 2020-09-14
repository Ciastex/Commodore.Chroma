using Chroma.Graphics;
using Commodore.EVIL;
using Commodore.EVIL.Abstraction;
using Commodore.EVIL.Exceptions;
using Commodore.EVIL.Execution;
using Commodore.EVIL.RuntimeLibrary.Base;
using Commodore.GameLogic.Core;

namespace Commodore.GameLogic.Executive.EvilRuntime
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

        public DynValue Margins(Interpreter interpreter, ClrFunctionArguments args)
        {
            if (args.Count == 0)
            {
                var tbl = new Table();
                
                tbl["left"] = new DynValue(Kernel.Instance.Vga.Margins.Left);
                tbl["top"] = new DynValue(Kernel.Instance.Vga.Margins.Top);
                tbl["right"] = new DynValue(Kernel.Instance.Vga.Margins.Right);
                tbl["bottom"] = new DynValue(Kernel.Instance.Vga.Margins.Bottom);
                
                return new DynValue(tbl);
            }
            else
            {
                args.ExpectExactly(4)
                    .ExpectByteAtIndex(0)
                    .ExpectByteAtIndex(1)
                    .ExpectByteAtIndex(2)
                    .ExpectByteAtIndex(3);

                Kernel.Instance.Vga.SetMargins(
                    (byte)args[0].Number,
                    (byte)args[1].Number,
                    (byte)args[2].Number,
                    (byte)args[3].Number
                );
                
                return DynValue.Zero;
            }
        }

        public DynValue SetXY(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(6)
                .ExpectTypeAtIndex(0, DynValueType.String)
                .ExpectIntegerAtIndex(1)
                .ExpectIntegerAtIndex(2)
                .ExpectByteAtIndex(3)
                .ExpectByteAtIndex(4)
                .ExpectByteAtIndex(5);

            var str = args[0].String;
            var x = args[1].Number;
            var y = args[2].Number;
            var r = (byte)args[3].Number;
            var g = (byte)args[4].Number;
            var b = (byte)args[5].Number;

            if (x >= Kernel.Instance.Vga.TotalColumns)
                return new DynValue(-1);

            if (y >= Kernel.Instance.Vga.TotalRows)
                return new DynValue(-1);

            if (str.Length != 1)
                throw new ClrFunctionException("Expected a single-character string.");

            Kernel.Instance.Vga.PutCharAt(str[0], new Color(r, g, b), Color.Black, (int)x, (int)y);
            return DynValue.Zero;
        }

        public DynValue SetCursorPosition(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(2)
                .ExpectIntegerAtIndex(0)
                .ExpectIntegerAtIndex(1);

            var x = args[0].Number;
            var y = args[1].Number;

            if (x >= Kernel.Instance.Vga.TotalColumns - Kernel.Instance.Vga.Margins.Right)
                x = Kernel.Instance.Vga.Margins.Right;

            if (y >= Kernel.Instance.Vga.TotalRows - Kernel.Instance.Vga.Margins.Bottom)
                y = Kernel.Instance.Vga.Margins.Bottom;

            if (x < Kernel.Instance.Vga.Margins.Left)
                x = Kernel.Instance.Vga.Margins.Left;

            if (y < Kernel.Instance.Vga.Margins.Top)
                y = Kernel.Instance.Vga.Margins.Top;
            
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
            env.RegisterBuiltIn("vga.margins", Margins);
        }
    }
}
