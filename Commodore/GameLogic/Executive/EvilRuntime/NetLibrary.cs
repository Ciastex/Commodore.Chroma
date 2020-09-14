using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Commodore.EVIL.Abstraction;
using Commodore.EVIL.Exceptions;
using Commodore.EVIL.Execution;
using Commodore.EVIL.RuntimeLibrary.Base;
using Commodore.GameLogic.Core;
using Commodore.GameLogic.Network;
using Commodore.GameLogic.Persistence;
using Environment = Commodore.EVIL.Environment;

namespace Commodore.GameLogic.Executive.EvilRuntime
{
    public class NetLibrary : ClrPackage
    {
        public DynValue ScanCurrentNeighborhood(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectNone();

            var tbl = new Table();

            var position = Vector2.Zero;

            if (Kernel.Instance.NetworkConnectionStack.TryPeek(out var entity))
            {
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

        public DynValue LinkToDevice(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(1)
                .ExpectTypeAtIndex(0, DynValueType.String);

            var addr = Address.Parse(args[0].String);
            var device = UserProfile.Instance.Internet.GetDevice(addr.ToStringWithoutNode());

            if (device == null)
                return new DynValue(SystemReturnCodes.Network.NoRouteToHost);

            if (Kernel.Instance.NetworkConnectionStack.TryPeek(out var entity))
            {
                if (entity is Node)
                    return new DynValue(SystemReturnCodes.Network.BoundToNode);
            }

            if (Kernel.Instance.NetworkConnectionStack.Contains(device))
                return new DynValue(SystemReturnCodes.Network.AlreadyLinked);


            Kernel.Instance.LinkToDevice(device);
            return new DynValue(SystemReturnCodes.Success);
        }

        public DynValue UnlinkFromDevice(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectNone();

            if (!Kernel.Instance.NetworkConnectionStack.TryPeek(out var entity))
                return new DynValue(SystemReturnCodes.Network.NotLinked);

            if (!(entity is Device device))
                return new DynValue(SystemReturnCodes.Network.NotADevice);

            if (!Kernel.Instance.CurrentSystemContext.IsLocal &&
                Kernel.Instance.CurrentSystemContext.RemoteDevice == device)
                return new DynValue(SystemReturnCodes.Network.ShellStillOpen);

            Kernel.Instance.UnlinkFromDevice();
            return new DynValue(SystemReturnCodes.Success);
        }

        public DynValue BindToNode(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(1)
                .ExpectByteAtIndex(0);

            var port = (byte)args[0].Number;

            if (!Kernel.Instance.NetworkConnectionStack.TryPeek(out var entity))
                return new DynValue(SystemReturnCodes.Network.NotLinked);

            if (!(entity is Device device))
                return new DynValue(SystemReturnCodes.Network.NotADevice);

            if (!Kernel.Instance.CurrentSystemContext.IsLocal &&
                Kernel.Instance.CurrentSystemContext.RemoteDevice == device)
                return new DynValue(SystemReturnCodes.Network.ShellStillOpen);

            if (!device.HasServiceNode(port))
                return new DynValue(SystemReturnCodes.Network.ServiceNodeUnresponsive);

            var node = device.GetServiceNode(port);

            if (!node.IsActive)
                return new DynValue(SystemReturnCodes.Network.ServiceNodeInactive);

            Kernel.Instance.BindToNode(node);
            return new DynValue(SystemReturnCodes.Success);
        }

        public DynValue UnbindFromNode(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectNone();

            if (!Kernel.Instance.NetworkConnectionStack.TryPeek(out var entity))
                return new DynValue(SystemReturnCodes.Network.NotLinked);

            if (!(entity is Node))
                return new DynValue(SystemReturnCodes.Network.NotANode);
            
            Kernel.Instance.UnbindFromNode();
            return new DynValue(SystemReturnCodes.Success);
        }

        public DynValue PingServiceNode(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(1)
                .ExpectByteAtIndex(0);

            var port = (byte)args[0].Number;

            if (!Kernel.Instance.NetworkConnectionStack.TryPeek(out var entity))
                return new DynValue(SystemReturnCodes.Network.NotLinked);

            if (!(entity is Device device))
                return new DynValue(SystemReturnCodes.Network.NotADevice);

            var serviceNode = device.GetServiceNode(port);

            if (serviceNode != null)
            {
                if (serviceNode.IsActive)
                    return new DynValue(SystemReturnCodes.Network.ServiceNodeActive);

                return new DynValue(SystemReturnCodes.Network.ServiceNodeInactive);
            }

            return new DynValue(SystemReturnCodes.Success);
        }
        
        public DynValue SendDataToBoundNode(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(1)
                .ExpectTypeAtIndex(0, DynValueType.Table);

            var data = args[0].Table;
            var bytes = new List<byte>();
            
            foreach (var dynVal in data.Values)
            {
                if (dynVal.Type == DynValueType.Number)
                    bytes.AddRange(BitConverter.GetBytes(dynVal.Number));
                else if (dynVal.Type == DynValueType.String)
                    bytes.AddRange(Encoding.ASCII.GetBytes(dynVal.String));
            }

            if (!Kernel.Instance.NetworkConnectionStack.TryPeek(out var entity))
                return new DynValue(SystemReturnCodes.Network.NotLinked);
            
            if(!(entity is Node node))
                return new DynValue(SystemReturnCodes.Network.NotANode);

            var response = node.GetResponse(bytes.ToArray());
            var ret = new Table();
            
            for (var i = 0; i < response.Length; i++)
                ret[i] = new DynValue(response[i]);

            return new DynValue(ret);
        }

        public DynValue AttachShellToDevice(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectNone();
            
            if (!Kernel.Instance.NetworkConnectionStack.TryPeek(out var entity))
                return new DynValue(SystemReturnCodes.Network.NotLinked);

            if (!(entity is Device device))
                return new DynValue(SystemReturnCodes.Network.NotADevice);

            if (!Kernel.Instance.CurrentSystemContext.IsLocal &&
                Kernel.Instance.CurrentSystemContext.RemoteDevice == device)
                return new DynValue(SystemReturnCodes.Network.ShellStillOpen);

            if (!device.ShellEnabled)
                return new DynValue(SystemReturnCodes.Network.ShellDisabled);

            Kernel.Instance.AttachShell(device);
            return new DynValue(SystemReturnCodes.Success);
        }

        public DynValue DetachShellFromDevice(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectNone();
            
            if (!Kernel.Instance.NetworkConnectionStack.TryPeek(out var entity))
                return new DynValue(SystemReturnCodes.Network.NotLinked);

            if (!(entity is Device))
                return new DynValue(SystemReturnCodes.Network.NotADevice);
            
            if(Kernel.Instance.CurrentSystemContext.IsLocal)
                return new DynValue(SystemReturnCodes.Network.ShellNotOpen);
            
            Kernel.Instance.DetachShell();
            return new DynValue(SystemReturnCodes.Success);
        }

        public DynValue GetHostName(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectNone();
            return new DynValue(Kernel.Instance.GetHostName());
        }

        public override void Register(Environment env, Interpreter interpreter)
        {
            env.RegisterBuiltIn("net.scan", ScanCurrentNeighborhood);
            env.RegisterBuiltIn("net.link", LinkToDevice);
            env.RegisterBuiltIn("net.unlink", UnlinkFromDevice);
            env.RegisterBuiltIn("net.ping", PingServiceNode);
            env.RegisterBuiltIn("net.bind", BindToNode);
            env.RegisterBuiltIn("net.unbind", UnbindFromNode);
            env.RegisterBuiltIn("net.send", SendDataToBoundNode);
            env.RegisterBuiltIn("net.attach", AttachShellToDevice);
            env.RegisterBuiltIn("net.detach", DetachShellFromDevice);
            env.RegisterBuiltIn("net.host", GetHostName);
        }
    }
}