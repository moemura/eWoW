using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using eWoW.Common;
using eWoW.Network.Cryptography;

namespace eWoW.Network
{
    public struct SocketData
    {
        public string AsString { get; set; }
        public byte[] AsByte { get; set; }
    }

    public class Server
    {
        #region Events & Handlers

        #region Delegates

        public delegate void OnConnectEventHandler(object sender, ClientConnection clientConnection);

        public delegate void OnDisconnectEventHandler(object sender, ClientConnection clientConnection);

        public delegate void OnRecieveEventHandler(object sender, ClientConnection clientConnection, Packet e);

        #endregion

        public event OnDisconnectEventHandler OnDisconnect;
        public event OnConnectEventHandler OnConnect;
        public event OnRecieveEventHandler OnRecieve;

        protected virtual void Receive(ClientConnection connection, Packet data)
        {
            if (OnRecieve != null)
            {
                OnRecieve(this, connection, data);
            }
        }

        protected virtual void Connect(ClientConnection connection)
        {
            if (OnConnect != null)
            {
                ConnectionPeak++;
                ConnectionUsers++;
                connection.Connected = true;

                OnConnect(this, connection);
            }
        }

        protected virtual void Disconnect(ClientConnection connection)
        {
            if (OnDisconnect != null)
            {
                ConnectionUsers--;
                connection.Connected = false;
                OnDisconnect(this, connection);
            }
        }

        #endregion

        protected const int MessageSize = 512000;
        private readonly int _port;
        private readonly TcpListener _tcpListener;
        private Thread _listenThread;

        public Server(int port)
        {
            _port = port;
            // Using any to avoid any confusion.
            _tcpListener = new TcpListener(IPAddress.Any, _port);
        }

        public int ConnectionPeak { get; protected set; }
        public int ConnectionUsers { get; protected set; }

        public void Start()
        {
            _listenThread = new Thread(ListenForClients);
            _listenThread.Start();
        }

        public void Stop()
        {
            _tcpListener.Stop();
            _listenThread.Abort();
            _listenThread = null;
        }

        private void ListenForClients()
        {
            _tcpListener.Start();

            while (true)
            {
                try
                {
                    TcpClient client = _tcpListener.AcceptTcpClient();

                    var clientThread = new Thread(HandleClientComm);
                    clientThread.Start(client);
                }
                catch (ThreadAbortException)
                {
                    // Handle our thread abort 'gracefully'
                    break;
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ConsoleColor.Red, ex);
                    throw;
                }
            }
        }

        private void HandleClientComm(object tcpClient)
        {
            var client = (TcpClient) tcpClient;
            NetworkStream clientStream = client.GetStream();

            var cc = new ClientConnection(ConnectionPeak, client, clientStream)
                         {ClientIp = client.Client.RemoteEndPoint as IPEndPoint};

            Connect(cc);

            while (true)
            {
                var message = new byte[MessageSize];
                int bytesRead;
                var socketData = new SocketData();

                try
                {
                    bytesRead = clientStream.Read(message, 0, MessageSize);
                }
                catch (Exception e)
                {
                    Disconnect(cc);
                    Logging.WriteException(ConsoleColor.Red, e);
                    break;
                }

                if (bytesRead == 0)
                {
                    Disconnect(cc);
                    break;
                }

                var tempData = new byte[bytesRead];

                for (int i = 0; i < bytesRead; i++)
                {
                    tempData[i] = message[i];
                }

                socketData.AsByte = tempData;
                socketData.AsString = Encoding.ASCII.GetString(message);

                Receive(cc, new Packet(socketData.AsByte));
            }

            client.Close();
        }

        #region Send Functions

        public void Send(ClientConnection conn, string data)
        {
            Send(conn, Encoding.ASCII.GetBytes(data));
        }

        public void Send(ClientConnection conn, byte[] data)
        {
            Send(conn, data, 0, data.Length);
        }

        public void Send(ClientConnection conn, byte[] data, int index, int count)
        {
            var _packetCrypto = new PacketCrypto(conn.Account.SessionKey);
            if (conn.Connected)
            {
                if (conn.Encrypted)
                {
                    _packetCrypto.Encrypt(data, index, count);
                }
                conn.Stream.Write(data, index, count);
                conn.Stream.Flush();
            }
        }

        #endregion
    }
}