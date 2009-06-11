using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using eWoW.Common;
using eWoW.Network;
using eWoW.Network.OpCodes;
using eWoW.Network.Packets;

namespace eWoW.LogonServer
{
    public enum AuthResults
    {
        REALM_AUTH_SUCCESS = 0x00,
        REALM_AUTH_FAILURE = 0x01,
        ///< Unable to connect
        REALM_AUTH_UNKNOWN1 = 0x02,
        ///< Unable to connect
        REALM_AUTH_ACCOUNT_BANNED = 0x03,
        ///< This <game> account has been closed and is no longer available for use. Please go to <site>/banned.html for further information.
        REALM_AUTH_NO_MATCH = 0x04,
        ///< The information you have entered is not valid. Please check the spelling of the account name and password. If you need help in retrieving a lost or stolen password, see <site> for more information
        REALM_AUTH_UNKNOWN2 = 0x05,
        ///< The information you have entered is not valid. Please check the spelling of the account name and password. If you need help in retrieving a lost or stolen password, see <site> for more information
        REALM_AUTH_ACCOUNT_IN_USE = 0x06,
        ///< This account is already logged into <game>. Please check the spelling and try again.
        REALM_AUTH_PREPAID_TIME_LIMIT = 0x07,
        ///< You have used up your prepaid time for this account. Please purchase more to continue playing
        REALM_AUTH_SERVER_FULL = 0x08,
        ///< Could not log in to <game> at this time. Please try again later.
        REALM_AUTH_WRONG_BUILD_NUMBER = 0x09,
        ///< Unable to validate game version. This may be caused by file corruption or interference of another program. Please visit <site> for more information and possible solutions to this issue.
        REALM_AUTH_UPDATE_CLIENT = 0x0a,
        ///< Downloading
        REALM_AUTH_UNKNOWN3 = 0x0b,
        ///< Unable to connect
        REALM_AUTH_ACCOUNT_FREEZED = 0x0c,
        ///< This <game> account has been temporarily suspended. Please go to <site>/banned.html for further information
        REALM_AUTH_UNKNOWN4 = 0x0d,
        ///< Unable to connect
        REALM_AUTH_UNKNOWN5 = 0x0e,
        ///< Connected.
        REALM_AUTH_PARENTAL_CONTROL = 0x0f
        ///< Access to this account has been blocked by parental controls. Your settings may be changed in your account preferences at <site>
    }

    public class Logon : Server
    {
        public List<Realm> Realms = new List<Realm>();

        public Logon() : base(3724)
        {
            OnConnect += Logon_OnConnect;
            OnDisconnect += Logon_OnDisconnect;
            OnRecieve += Logon_OnRecieve;
        }

        #region Server Event Handlers

        private void Logon_OnRecieve(object sender, ClientConnection clientConnection, Packet e)
        {
            var AuthOpCode = (AuthServerOpCode) e.Reader.ReadOpCode(PacketHeaderType.AuthCmsg);
            switch (AuthOpCode)
            {
                case AuthServerOpCode.AUTH_LOGON_CHALLENGE:
                    HandleAuthLogonChallenge(clientConnection, e);
                    break;
                case AuthServerOpCode.AUTH_LOGON_PROOF:
                    HandleAuthLogonProof(clientConnection, e);
                    break;
                    //case AuthServerOpCode.AUTH_RECONNECT_CHALLENGE:
                    //    HandleAuthReconnectChallenge(clientConnection, e);
                    //    break;
                    //case AuthServerOpCode.AUTH_RECONNECT_PROOF:
                    //    HandleAuthReconnectProof(clientConnection, e);
                    //    break;
                case AuthServerOpCode.REALM_LIST:
                    HandleRealmList(clientConnection, e);
                    break;
                default:
                    Logging.WriteDebug(ConsoleColor.Red,
                                       "UNKNOWN OPCODE RECEIVED IN LOGIN_RECEIVE HANDLER: " + AuthOpCode);
                    break;
            }
        }

        private void Logon_OnDisconnect(object sender, ClientConnection clientConnection)
        {}

        private void Logon_OnConnect(object sender, ClientConnection clientConnection)
        {}

        #endregion

        private void HandleRealmList(ClientConnection connection, Packet inPacket)
        {
            var outPacket = new RealmPacket(AuthServerOpCode.REALM_LIST);

            outPacket.Writer.Write(0);
            outPacket.Writer.Write((ushort) Realms.Count);
            foreach (Realm r in Realms)
            {
                outPacket.Writer.Write((byte) r.ServerType);
                outPacket.Writer.Write((byte) r.ServerStatus);
                outPacket.Writer.Write((byte) r.Color);

                outPacket.Writer.Write(r.RealmName);
                outPacket.Writer.Write();

                outPacket.Writer.Write(r.Ip.ToString());
                outPacket.Writer.Write();

                outPacket.Writer.Write(r.Population);

                outPacket.Writer.Write();

                outPacket.Writer.Write((byte) r.TimeZone);
                outPacket.Writer.Write();
            }

            outPacket.Writer.Write((byte) 0x17);
            outPacket.Writer.Write();

            Send(connection, outPacket.Data);
        }

        private void HandleAuthLogonProof(ClientConnection connection, Packet inPacket)
        {
            var outPacket = new RealmPacket(AuthServerOpCode.AUTH_LOGON_PROOF);
            var a = new byte[32];
            var m1 = new byte[20];

            Array.Copy(inPacket.Data, 1, a, 0, 32);
            Array.Copy(inPacket.Data, 1, m1, 0, 20);

            Account acc = Account.Find(connection);
            acc.Srp6.CalculateU(a);
            acc.Srp6.CalculateM2(m1);

            CalculateAccountHash(connection.Account);

            // TODO: Is this right?
            if (!IsCorrectHash(acc, a, m1))
            {
                outPacket.Writer.Write((byte) AccountStatus.UnknownAccount);
            }
            else
            {
                outPacket.Writer.Write((byte) AccountStatus.Ok);
            }

            outPacket.Writer.Write(acc.Srp6.M2);

            //write 10 null bytes
            outPacket.Writer.WriteNull(10);

            Send(connection, outPacket.Data);
        }

        private void HandleAuthLogonChallenge(ClientConnection connection, Packet inPacket)
        {
            Logging.WriteDebug("HandleAuthLogonChallenge entered (" + connection.ClientIp + ")");
            inPacket.Reader.BaseStream.Position += 8;

            // Because this is just plain easier to work with, and we can pass it around as much as we want.
            var version = new Version(inPacket.Data[9],
                                      BitConverter.ToInt16(new[] {inPacket.Data[11], inPacket.Data[12]}, 0),
                                      inPacket.Data[10]);

            //Before we go any further, make sure the user has a valid game version for this server.
            //if (version.Major > Config.GetValue<uint>(Config.LogonServerConfig, "Max"))
            // {
            //     SendChallengeError(connection, AccountStatus.BadVersion);
            //     return;
            // }
            // else if (version.Minor < Config.GetValue<uint>(Config.LogonServerConfig, "Min"))
            // {
            //     //TODO: Initialize patching sequence!... then comment these two lines out.
            //    SendChallengeError(connection, AccountStatus.BadVersion);
            //    return;
            // }

            IPAddress ip = inPacket.Reader.ReadIpAddress(); // 4 bytes [0x1D-0x21]

            // Uncomment/change the following line if the byte for the string length changes.
            //inPacket.Reader.BaseStream.Seek(33, SeekOrigin.Begin);

            // Account name *should* be in upper case, but may not.
            string accountName = inPacket.Reader.ReadPascalString(1);

            Logging.WriteDebug("Logon Challenge: Client " + version);
            Logging.WriteDebug("Account: " + accountName + " IP: " + ip);

            var acc = new Account();

            if (Account.Exists(accountName))
            {
                DataTable dt = Account.QueryAccountInfo(accountName);

                acc.Username = dt.Rows[0]["login"].ToString();
                acc.Password = dt.Rows[0]["password"].ToString();

                byte[] userBytes = Encoding.UTF8.GetBytes(acc.Username.ToUpper());
                byte[] passBytes = Encoding.UTF8.GetBytes(acc.Password.ToUpper());

                acc.Srp6.CalculateX(userBytes, passBytes);

                acc.IP = ip;
                acc.ID = connection.ClientId;

                var outPacket = new RealmPacket(AuthServerOpCode.AUTH_LOGON_CHALLENGE);
                outPacket.Writer.Write();
                outPacket.Writer.Write((byte) AccountStatus.Ok);
                outPacket.Writer.Write(acc.Srp6.B);
                outPacket.Writer.Write((byte) 1);
                outPacket.Writer.Write(acc.Srp6.g[0]);
                outPacket.Writer.Write((byte) acc.Srp6.N.Length);
                outPacket.Writer.Write(acc.Srp6.N);
                outPacket.Writer.Write(acc.Srp6.salt);

                outPacket.Writer.WriteNull(17);

                Send(connection, outPacket.Data);
                connection.Account = acc;
                ClientConnection.Connections.Add(connection);
            }
            else
            {
                SendProofError(connection, AccountStatus.UnknownAccount);
                return;
            }
        }

        #region Helper Functions

        private void SendChallengeError(ClientConnection connection, AccountStatus error)
        {
            //Could add a ctor or change the packet class around, but this is easier.
            var data = new byte[3];
            data[0] = 0x00;
            data[1] = 0x00;
            data[2] = (byte) error;

            Send(connection, data);
        }

        private void SendProofError(ClientConnection connection, AccountStatus error)
        {
            var data = new byte[3];
            data[0] = (byte) error;
            data[1] = 0x03;
            data[2] = 0x00;

            Send(connection, data);
        }

        private static bool IsCorrectHash(Account acc, byte[] a, byte[] m1)
        {
            SHA1 hash = new SHA1Managed();
            byte[] nHash = hash.ComputeHash(acc.Srp6.N);
            byte[] gHash = hash.ComputeHash(acc.Srp6.g);
            byte[] userHash = hash.ComputeHash(Encoding.UTF8.GetBytes(acc.Username));

            var ngHash = new byte[20];
            for (int i = 0; i < 20; i++)
            {
                ngHash[i] = (byte) (nHash[i] ^ gHash[i]);
            }

            // Lots of 'Append'ing
            byte[] appended = Append(Append(Append(ngHash, userHash), Append(acc.Srp6.salt, a)),
                                     Append(acc.Srp6.B, acc.Srp6.K));

            byte[] hashed = hash.ComputeHash(appended);

            for (int i = 0; i < 20; i++)
            {
                if (hashed[i] != m1[i])
                {
                    return false;
                }
            }
            return true;
        }

        private static byte[] Append(byte[] buf1, byte[] buf2)
        {
            var result = new byte[buf1.Length + buf2.Length];
            Buffer.BlockCopy(buf1, 0, result, 0, buf1.Length);
            Buffer.BlockCopy(buf2, 0, result, buf1.Length, buf2.Length);
            return result;
        }

        public static byte[] Concat(byte[] a, byte[] b)
        {
            var res = new byte[a.Length + b.Length];
            for (int t = 0; t < a.Length; t++)
            {
                res[t] = a[t];
            }
            for (int t = 0; t < b.Length; t++)
            {
                res[t + a.Length] = b[t];
            }
            return res;
        }

        public static void CalculateAccountHash(Account acct)
        {
            SHA1 shaM1 = new SHA1CryptoServiceProvider();
            byte[] S = acct.Srp6.S;
            var S1 = new byte[16];
            var S2 = new byte[16];

            for (int t = 0; t < 16; t++)
            {
                S1[t] = S[t * 2];
                S2[t] = S[(t * 2) + 1];
            }

            byte[] hashS1 = shaM1.ComputeHash(S1);
            byte[] hashS2 = shaM1.ComputeHash(S2);
            acct.SessionKey = new byte[hashS1.Length + hashS2.Length];
            for (int t = 0; t < 20; t++)
            {
                acct.SessionKey[t * 2] = hashS1[t];
                acct.SessionKey[(t * 2) + 1] = hashS2[t];
            }

            var opad = new byte[64];
            var ipad = new byte[64];

            //Static 16 byte Key located at 0x0088FB3C
            var key = new byte[] {56, 167, 131, 21, 248, 146, 37, 48, 113, 152, 103, 177, 140, 4, 226, 170};

            //Fill 64 bytes of same value
            for (int i = 0; i <= 64 - 1; i++)
            {
                opad[i] = 0x05C;
                ipad[i] = 0x036;
            }

            //XOR Values
            for (int i = 0; i <= 16 - 1; i++)
            {
                opad[i] = (byte) (opad[i] ^ key[i]);
                ipad[i] = (byte) (ipad[i] ^ key[i]);
            }

            byte[] buffer1 = Concat(ipad, acct.SessionKey);
            byte[] buffer2 = shaM1.ComputeHash(buffer1);

            buffer1 = Concat(opad, buffer2);
            acct.SessionKey = shaM1.ComputeHash(buffer1);
        }

        #endregion

        //        //private void HandleAuthReconnectChallenge(ClientConnection c, Packet packet)
        //        //{
        //        //    //TODO: packet param not used, safe to remove?
        //        //    var p = new AuthPacket(AuthServerOpCode.AUTH_RECONNECT_CHALLENGE);
        //        //    //p.Write((uint) AuthServerOpCode.AUTH_RECONNECT_CHALLENGE);
        //        //    p.WriteNullBytes(1);
        //        //    p.Write(Account.Find(c).Srp6.K);
        //        //    p.WriteNullBytes(17);

        //        //    Send(c, p.GetOutPacket());
        //        //}

        //        //private void HandleAuthReconnectProof(ClientConnection connection, Packet packet)
        //        //{
        //        //    //TODO: packet param not used, safe to remove?
        //        //    var p = new Packet();
        //        //    p.Write(new byte[] {0x03, 0x00});
        //        //    Send(connection, p.GetOutPacket());
        //        //}
    }

    //    struct AUTH_LOGON_CHALLENGE_CLIENT
    //    {
    //        public byte Command;
    //        public byte Error;
    //        public ushort Size;

    //        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
    //        public string GameLocale;

    //        public byte VersionMajor;
    //        public byte VersionMinor;
    //        public byte VersionRevision;
    //        public ushort VersionBuild;
    //        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
    //        public string Platform;

    //        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
    //        public string OS;
    //        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
    //        public string Country;

    //        public uint Timezone;
    //        public uint IP;
    //        public byte I_len;

    //    }
}