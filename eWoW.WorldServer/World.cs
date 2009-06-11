using System;
using eWoW.Common;
using eWoW.Network;
using eWoW.Network.OpCodes;
using eWoW.Network.Packets;

namespace eWoW.WorldServer
{
    public class World : Server
    {
        /// <summary>
        /// Random generated uint used in the authentication process of SMSG_AUTH_CHALLENGE
        /// </summary>
        private readonly int _connectionSeed = new Random().Next();

        //private readonly byte[] _connectionSeed = BitConverter().GetBytes(new Random().Next());

        public World() : base(8085)
        {
            OnConnect += World_OnConnect;
            OnDisconnect += World_OnDisconnect;
            OnRecieve += World_OnRecieve;
        }

        private void World_OnRecieve(object sender, ClientConnection clientConnection, Packet e)
        {
            Logging.WriteDebug("FROM WORLD");
            //HexViewer.View(e.AsByte, 0, e.AsByte.Length);
            /*PacketCrypto _packetCrypto = new PacketCrypto(clientConnection.Account.AccountHash);
            if (clientConnection.Encrypted)
            {
                _packetCrypto.Decrypt(e, 0, e.Data.Length);
            }*/

            switch ((RealmServerOpCode) e.OpCode)
            {
                case RealmServerOpCode.CMSG_AUTH_SESSION: //move this into a new .cs?
                    e.Writer.BaseStream.Position += 8;
                    clientConnection.Account.Username = e.Reader.ReadCString();
                    uint unk302 = e.Reader.ReadUInt32();
                    clientConnection.Account.SessionID = e.Reader.ReadBytes(4);
                    var p = new WorldPacket(RealmServerOpCode.SMSG_AUTH_RESPONSE);
                    p.Writer.Write(0x0C);
                    p.Writer.Write();
                    p.Writer.Write((byte) 2); //billing plan flags
                    p.Writer.Write(); //billing plan rested
                    p.Writer.Write(); //Expansion
                    Send(clientConnection, p.GetCompressedOutPacket());
                    //Addon packet info will go here

                    /*List<String> AddonNames = new List<String>();
                    List<uint> AddonHashes = new List<uint>();
                    uint AddonCount = e.Reader.ReadUInt32();
                    for (int i = 0; i < AddonCount; i++)
                    {
                        AddonNames.Add(e.Reader.ReadCString());
                        e.Reader.ReadByte();
                        AddonHashes.Add(e.Reader.ReadUInt32());
                        e.Reader.ReadUInt32();
                    }

                    var addon = new Network.Packets.WorldPacket(RealmServerOpCode.SMSG_ADDON_INFO);
                    for(int i = 0; i < AddonNames.Count; i++)
                    {
                        addon.Writer.Write((byte)2); // addon type
                        addon.Writer.Write((byte)1); //unk
                        addon.Writer.Write(); // hash
                        addon.Writer.Write((UInt32)0);
                        addon.Writer.Write(); //3.0.8 unknown
                    }
                    Send(clientConnection, addon.GetCompressedOutPacket());*/
                    break;
                default:
                    PacketHandler.RunIncoming(clientConnection, e);
                    break;
            }
        }

        private void World_OnDisconnect(object sender, ClientConnection clientConnection)
        {
            //throw new NotImplementedException();
        }

        private void World_OnConnect(object sender, ClientConnection clientConnection)
        {
            Logging.WriteDebug("WORLD CONNECT");
            var p = new WorldPacket(RealmServerOpCode.SMSG_AUTH_CHALLENGE);
            p.Writer.Write(_connectionSeed);
            //var p = new Packet();
            //p.Write(RealmServerOpCode.SMSG_AUTH_CHALLENGE);
            //p.Write(_connectionSeed);
            Send(clientConnection, p.GetCompressedOutPacket());
        }
    }
}