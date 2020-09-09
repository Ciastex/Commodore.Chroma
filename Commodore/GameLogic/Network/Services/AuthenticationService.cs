using System;
using System.IO;
using System.Text;
using Commodore.Framework;
using Commodore.Framework.Generators;
using Commodore.GameLogic.Generation;

namespace Commodore.GameLogic.Network.Services
{
    public class AuthenticationService : Service
    {
        private const byte CMD_GETSALT = 0x01;
        private const byte CMD_SETUSR = 0x02;
        private const byte CMD_SETPWD = 0x03;
        private const byte CMD_AUTH = 0x04;

        private const byte RSP_NOK = 0x00;
        private const byte RSP_OK = 0x01;

        private const byte RSN_CMD_UNKNOWN = 0x01;
        private const byte RSN_USR_BAD = 0x02;
        private const byte RSN_PWD_BAD = 0x03;

        private string AuthUserName { get; set; }
        private string AuthPassword { get; set; }

        public override string Banner => "AUTHSERV_1.0";

        public byte Salt { get; } = (byte)G.Random.Next(0, 128);
        public string Password { get; } = StringGenerator.GenerateRandomAlphanumericString(4);
        public string UserName { get; } = UsernameGenerator.Generate();

        public bool IsAuthenticated { get; private set; }

        public override byte[] GetResponse(byte[] data)
        {
            using var outms = new MemoryStream();

            try
            {
                using var br = new BinaryReader(new MemoryStream(data));
                using var bw = new BinaryWriter(outms);

                var cmd = br.ReadByte();

                switch (cmd)
                {
                    case CMD_GETSALT:
                        bw.Write(RSP_OK);
                        bw.Write(Salt);
                        break;

                    case CMD_SETUSR:
                        var usrLen = br.ReadByte();
                        AuthUserName = Encoding.ASCII.GetString(br.ReadBytes(usrLen));
                        bw.Write(RSP_OK);
                        break;

                    case CMD_SETPWD:
                        var pwdLen = br.ReadByte();
                        AuthPassword = Encoding.ASCII.GetString(br.ReadBytes(pwdLen));
                        bw.Write(RSP_OK);
                        break;

                    case CMD_AUTH:
                        if (string.Compare(Password, AuthPassword, StringComparison.Ordinal) != 0)
                        {
                            bw.Write(RSP_NOK);
                            bw.Write(RSN_PWD_BAD);
                            break;
                        }

                        if (string.Compare(UserName, AuthUserName, StringComparison.Ordinal) != 0)
                        {
                            bw.Write(RSP_NOK);
                            bw.Write(RSN_USR_BAD);
                            break;
                        }

                        bw.Write(RSP_OK);
                        IsAuthenticated = true;
                        break;
                }
            }
            catch
            {
                return new byte[] {0xFF, RSN_CMD_UNKNOWN};
            }

            return outms.ToArray();
        }
    }
}