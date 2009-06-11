using System;
using System.Net;
using System.Xml.Linq;

namespace eWoW.LogonServer
{
    public enum RealmColor : byte
    {
        Green = 0x0,
        Red = 0x1,
        Offline = 0x2,
    }

    public enum RealmTimeZone : byte
    {
        Development = 1,
        USA = 2,
        Oceanic = 3,
        LatinAmerica = 4,
        Tournament = 5,
        Korea = 6,
        Tournament2 = 7,
        English = 8,
        German = 9,
        French = 10,
        Spanish = 11,
        Russian = 12,
        Tournament3 = 13,
        Taiwan = 14,
        Tournament4 = 15,
        China = 16,
        CN1 = 17,
        CN2 = 18,
        CN3 = 19,
        CN4 = 20,
        CN5 = 21,
        CN6 = 22,
        CN7 = 23,
        CN8 = 24,
        Tournament5 = 25,
        TestServer = 26,
        Tournament6 = 27,
        QAServer = 28,
        CN9 = 29,
        TestServer2 = 30
    }

    public enum RealmStatus : byte
    {
        Good = 0x00,
        Locked = 0x01,
    }

    public enum RealmType : byte
    {
        Normal = 0x00,
        PVP = 0x01,
        RP = 0x06,
        RPPVP = 0x08,
    }

    public class Realm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Realm(string realmName, RealmType serverType, RealmStatus serverStatus, RealmTimeZone timeZone,
                     RealmColor color, int maxPlayers)
        {
            RealmName = realmName;
            ServerType = serverType;
            ServerStatus = serverStatus;
            TimeZone = timeZone;
            Color = color;
            MaxPlayers = maxPlayers;
        }

        public Realm(XElement configElement)
        {
            throw new NotImplementedException();
        }

        public string RealmName { get; set; }

        public RealmType ServerType { get; set; }
        public RealmStatus ServerStatus { get; set; }
        public RealmTimeZone TimeZone { get; set; }
        public RealmColor Color { get; set; }
        public int MaxPlayers { get; set; }

        public float Population
        {
            get
            {
                float z = 100 / ((float) MaxPlayers);
                if (z <= 33)
                {
                    return 0.5f;
                }
                if (z > 33 && z <= 66)
                {
                    return 1.0f;
                }
                return z > 66 ? 2.0f : 0;
            }
        }

        public IPAddress Ip { get; set; }
        public int Port { get; set; }
    }
}