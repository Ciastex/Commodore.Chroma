using Commodore.Engine;
using Commodore.EVIL;
using Commodore.EVIL.Abstraction;
using Commodore.EVIL.Execution;
using Commodore.EVIL.RuntimeLibrary.Base;
using Commodore.GameLogic.Persistence;
using System.Diagnostics;
using System.Linq;

namespace Commodore.GameLogic.Executive.ComplexRuntime
{
    public class NetLibrary : ClrPackage
    {
        public DynValue Scan(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectNone();

            var table = new Table();

            var linkables = UserProfile.Instance.Network.Cursor.CurrentConnectedDevice?.LinkableNeighbours ??
                            UserProfile.Instance.Network.Cursor.Gateway.LinkableNeighbours;

            for (var i = 0; i < linkables.Count; i++)
            {
                table[i] = new DynValue(linkables[i].IP.Address);
            }

            return new DynValue(table);
        }

        public DynValue Link(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(1)
                .ExpectTypeAtIndex(0, DynValueType.String);

            var ip = args[0].String;
            var linkables = UserProfile.Instance.Network.Cursor.CurrentConnectedDevice?.LinkableNeighbours ??
                            UserProfile.Instance.Network.Cursor.Gateway.LinkableNeighbours;

            var linkable = linkables.FirstOrDefault(x => x.IP.Address == ip);

            if (linkable == null)
                return new DynValue(0);

            var stp = new Stopwatch();
            stp.Start();
            UserProfile.Instance.Network.Cursor.LinkTo(linkable);
            stp.Stop();
            DebugLog.Info(stp.ElapsedMilliseconds.ToString());

            return new DynValue(1);
        }

        public DynValue Unlink(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectNone();

            if (UserProfile.Instance.Network.Cursor.CurrentConnectedDevice == null)
                return new DynValue(0);

            UserProfile.Instance.Network.Cursor.Unlink();
            return new DynValue(1);
        }

        public DynValue Host(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectNone();

            if (UserProfile.Instance.Network.Cursor.CurrentConnectedDevice == null)
                return new DynValue("localhost");

            return new DynValue(UserProfile.Instance.Network.Cursor.CurrentConnectedDevice.IP.Address);
        }

        public override void Register(Environment env, Interpreter interpreter)
        {
            env.RegisterBuiltIn("net.scan", Scan);
            env.RegisterBuiltIn("net.link", Link);
            env.RegisterBuiltIn("net.unlink", Unlink);
            env.RegisterBuiltIn("net.host", Host);
        }
    }
}
