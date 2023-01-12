using System;
using Commodore.Framework.Generators;
using Commodore.GameLogic.Network.Nodes;

namespace Commodore.GameLogic.Network.Devices
{
    [Serializable]
    public class PersonalComputer : Device
    {
        public PersonalComputer()
        {
            CreateServiceNode<SimpleAuthNode>(1);

            var homeDir = RootDirectory.CreateGivenPath("/home");

            for (var i = 0; i < 16; i++)
            {
                homeDir.AddNewFile(StringGenerator.GenerateRandomAlphanumericString(6) + ".dat")
                    .SetData(StringGenerator.GenerateRandomAlphanumericString(128));
            }

            RootDirectory.CreateGivenPath("/bin");
            var etcDir = RootDirectory.CreateGivenPath("/etc/");
            etcDir.AddNewFile("hostname")
                .SetData("openinside");

            etcDir.AddNewFile("passwd")
                .SetData("root:toor");
        }
    }
}