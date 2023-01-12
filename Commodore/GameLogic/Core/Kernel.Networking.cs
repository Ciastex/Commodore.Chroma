using System.Threading.Tasks;
using Commodore.GameLogic.Network;
using Commodore.GameLogic.Persistence;

namespace Commodore.GameLogic.Core
{
    public partial class Kernel
    {
        public void LinkToDevice(Device device)
        {
            if (device == null)
                return;

            NetworkConnectionStack.Push(device);
            device.OnLinked();
        }

        public void UnlinkFromDevice()
        {
            if (NetworkConnectionStack.TryPeek(out var entity))
            {
                if (entity is Device device)
                {
                    NetworkConnectionStack.Pop();
                    device.OnUnlinked();
                }
            }
        }

        public void BindToNode(Node node)
        {
            NetworkConnectionStack.Push(node);
            node.OnBound();
        }

        public void UnbindFromNode()
        {
            if (NetworkConnectionStack.TryPeek(out var entity))
            {
                if (entity is Node node)
                {
                    NetworkConnectionStack.Pop();
                    node.OnUnbound();
                }
            }
        }

        public void AttachShell(Device device)
        {
            CurrentSystemContext = device.SystemContext;
            device.OnShellAttached();
        }

        public void DetachShell()
        {
            var device = CurrentSystemContext.RemoteDevice;
            CurrentSystemContext = LocalSystemContext;

            device.OnShellDetached();
        }

        public bool IsShellAttached()
        {
            return !CurrentSystemContext.IsLocal;
        }

        public string GetHostName()
        {
            if (CurrentSystemContext.IsLocal)
            {
                try
                {
                    return LocalSystemContext.RootDirectory.Subdirectory("etc")
                        .File("hostname")
                        .GetData();
                }
                catch
                {
                    return "localhost";
                }
            }
            else
            {
                return CurrentSystemContext.RemoteDevice.GetHostName();
            }
        }

        private void StartNetworkUpdates()
        {
            Task.Run(async () =>
            {
                while (!IsRebooting)
                {
                    if (!UserProfile.Instance.IsInitialized)
                        continue;

                    if (UserProfile.Instance.Internet == null)
                        continue;

                    await UserProfile.Instance.Internet.Tick();
                }
            });
        }
    }
}