using eWoW.Network.OpCodes;

namespace eWoW.Network.Packets
{
    public class RealmPacket : Packet
    {
        public RealmPacket(byte[] data) : base(data)
        {}

        public RealmPacket(AuthServerOpCode opCode) : base(opCode.Parse(), (ushort) opCode)
        {}
    }
}