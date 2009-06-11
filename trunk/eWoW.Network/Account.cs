using System;
using System.Data;
using System.Net;
using eWoW.Common;
using eWoW.Database;
using eWoW.Network.Cryptography;

namespace eWoW.Network
{
    /// <summary>
    /// Realm Auth Proof Error Codes
    /// </summary>
    public enum AccountStatus : byte
    {
        Ok = 0x00,               // Successfully logged in.
        IPBanned = 0x01,         // "Unable to connect"
        Banned = 0x03,           // "This World of Warcraft account has been closed and is no longer in service -- Please check the registered email address of this account for further information."
        UnknownAccount = 0x04,
        AlreadyOnline = 0x06,    // "This account is already logged into World of Warcraft. Please check the spelling and try again."
        NoTimeLeft = 0x07,       // "You have used up your prepaid time for this account. Please purchase more to continue playing"
        DBBusy = 0x08,           // "Could not log in to World of Warcraft at this time. Please try again later."
        BadVersion = 0x09,       // "Unable to validate game version. This may be caused by file corruption or the interference of another program.  Please visit www.blizzard.com/support/wow/ for more information and possible solutions to this issue."
        DownloadFile = 0x0A,     // Not official name
        AccountFrozen = 0x0C,
        ParentalControl = 0x0F   // "Access to this account has been blocked by parental controls.  Your settings may be changed in your account preferences at http://www.worldofwarcraft.com."
    }

    public class Account
    {
        public Account()
        {
            Srp6 = new SRP6(true);
        }

        public int ID { get; set; }
        public int GM { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public byte[] SessionID { get; set; }
        public SRP6 Srp6 { get; set; }
        public IPAddress IP { get; set; }
        public int Port { get; set; }
        public byte[] SessionKey { get; set; }

        public static Account Find(ClientConnection c)
        {
            ClientConnection conn = ClientConnection.Connections.Find(cc => cc == c);
            return conn != null ? conn.Account : null;
        }

        public static bool Exists(string accountName)
        {
            return QueryAccountInfo(accountName) != null;
        }

        public static DataTable QueryAccountInfo(string accountName)
        {
            DataTable query = null;
            try
            {
                query =
                    SqlBase.Instance("LogonSettings").Query(string.Format("SELECT * FROM `accounts` WHERE `login` = '{0}'", accountName));
                if (query.Rows.Count > 0)
                {
                    return query;
                }
                return null;
            }
            catch (Exception e)
            {
                Logging.WriteException(e);
                return null;
            }
        }

        public static void CreateNew(string name, string password)
        {
            SqlBase.Instance("LogonSettings").Execute(
                string.Format(
                    "INSERT INTO `accounts` (`login`, `password`, `banned`, `flags`) VALUES ('{0}', '{1}', '0', '44')",
                    name, password));
        }
    }
}