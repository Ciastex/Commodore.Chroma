using System.Linq;
using System.Numerics;
using Commodore.EVIL;
using Commodore.EVIL.Abstraction;
using Commodore.EVIL.Execution;
using Commodore.EVIL.RuntimeLibrary.Base;
using Commodore.GameLogic.Core;
using Commodore.GameLogic.Network;
using Commodore.GameLogic.Persistence;

namespace Commodore.GameLogic.Executive.EvilRuntime
{
    public class NetLibrary : ClrPackage
    {
        public DynValue ScanCurrentNeighborhood(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectNone();

            var tbl = new Table();

            var position = Vector2.Zero;

            if (Kernel.Instance.NetworkConnectionStack.Any())
            {
                var entity = Kernel.Instance.NetworkConnectionStack.Peek();

                if (entity is Device dev)
                {
                    position = dev.GetNetPositionVector();
                }
                else if (entity is Node node)
                {
                    position = node.Owner.GetNetPositionVector();
                }
            }

            var devices = UserProfile.Instance.Internet.GetDevicesInRange(position, 10);

            for (var i = 0; i < devices.Count; i++)
                tbl[i] = new DynValue(devices[i].Address.Value.ToString());

            return new DynValue(tbl);
        }

        public DynValue PingServiceNode(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(2)
                .ExpectTypeAtIndex(0, DynValueType.String)
                .ExpectByteAtIndex(1);

            var device = UserProfile.Instance.Internet.GetDevice(args[0].String);
            var port = (byte)args[1].Number;

            if (device == null)
                return new DynValue(-1);

            var serviceNode = device.GetServiceNode(port);

            if (serviceNode != null)
            {
                if (serviceNode.IsActive)
                {
                    return new DynValue(1);
                }
                else return new DynValue(2);
            }

            return new DynValue(0);
        }

        public override void Register(Environment env, Interpreter interpreter)
        {
            env.RegisterBuiltIn("net.scan", ScanCurrentNeighborhood);
            env.RegisterBuiltIn("net.ping", PingServiceNode);
        }
    }
}