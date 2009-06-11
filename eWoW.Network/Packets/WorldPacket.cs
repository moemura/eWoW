using eWoW.Network.OpCodes;

namespace eWoW.Network.Packets
{
    public class WorldPacket : Packet
    {
        public WorldPacket(byte[] data) : base(data)
        {}

        public WorldPacket(RealmServerOpCode opCode) : base(opCode.Parse(), (ushort) opCode)
        {}
    }
}