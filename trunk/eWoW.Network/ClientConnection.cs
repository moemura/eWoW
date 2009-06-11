using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace eWoW.Network
{
    public class ClientConnection
    {
        private static readonly List<ClientConnection> _connections = new List<ClientConnection>();

        public ClientConnection(int id, TcpClient client, NetworkStream stream)
        {
            ClientId = id;
            TcpClient = client;
            Stream = stream;
            Connected = true;
        }

        public static List<ClientConnection> Connections { get { return _connections; } }

        public int ClientId { get; set; }
        public IPEndPoint ClientIp { get; set; }

        public bool Connected { get; set; }
        public bool Encrypted { get { return Account.SessionKey != null; } }
        public TcpClient TcpClient { get; set; }
        public NetworkStream Stream { get; set; }

        public Account Account { get; set; }
    }
}