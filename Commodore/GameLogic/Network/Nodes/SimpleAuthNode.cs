using System;
using System.IO;
using System.Text;
using Directory = Commodore.GameLogic.Core.IO.Storage.Directory;

namespace Commodore.GameLogic.Network.Nodes
{
    [Serializable]
    public class SimpleAuthNode : Node
    {
        [Serializable]
        public class Commands
        {
            public const byte LogIn = 0x01;
            public const byte LogOut = 0x02;
        }

        [Serializable]
        public class Responses
        {
            public const byte Success = 0x00;
            public const byte InvalidData = 0x01;
            public const byte InvalidUsername = 0x02;
            public const byte InvalidPassword = 0x03;
            public const byte InvalidCommand = 0x04;
            public const byte Unauthorized = 0x05;
        }

        public override string Banner => "SIMPLE_AUTH:1.0";

        public string Username
        {
            get
            {
                try
                {
                    return Owner.RootDirectory
                        .Subdirectory("etc")
                        .File("passwd")
                        .GetData()
                        .Split(':')[0];
                }
                catch
                {
                    return "admin";
                }
            }
        }

        public string Password
        {
            get
            {
                try
                {
                    return Owner.RootDirectory
                        .Subdirectory("etc")
                        .File("passwd")
                        .GetData()
                        .Split(':')[1];
                }
                catch
                {
                    return "default";
                }
            }
        }

        public SimpleAuthNode(Device device) : base(device)
        {
            Owner.ShellEnabled = false;
        }

        public override byte[] GetResponse(byte[] data)
        {
            if (data.Length == 0)
                return Data(Responses.InvalidData);

            using var ms = new MemoryStream(data);
            using var br = new BinaryReader(ms);

            var command = br.ReadByte();

            switch (command)
            {
                case Commands.LogIn:
                    try
                    {
                        var usernameLength = br.ReadByte();
                        var passwordLength = br.ReadByte();

                        var username = Encoding.ASCII.GetString(br.ReadBytes(usernameLength));
                        var password = Encoding.ASCII.GetString(br.ReadBytes(passwordLength));

                        if (username != Username)
                            return Data(Responses.InvalidUsername);

                        if (password != Password)
                            return Data(Responses.InvalidPassword);

                        Owner.ShellEnabled = true;
                        return Data(Responses.Success);
                    }
                    catch
                    {
                        return Data(Responses.InvalidData);
                    }

                case Commands.LogOut:
                    if (!Owner.ShellEnabled)
                        return Data(Responses.Unauthorized);

                    Owner.ShellEnabled = false;
                    return Data(Responses.Success);

                default: return Data(Responses.InvalidCommand);
            }
        }
    }
}