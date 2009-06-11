using eWoW.Network.OpCodes;

namespace eWoW.Network.Packets
{
    /// <summary>
    /// The header type of a packet. Values correspond to the total byte size of their headers.
    /// </summary>
    public enum PacketHeaderType : byte
    {
        AuthCmsg = 1,
        AuthSmsg = 3,
        WorldSmsg = 4,
        WorldCmsg = 6
    }

    public static class PacketHeaderExtension
    {
        public static PacketHeaderType Parse(this RealmServerOpCode opCode)
        {
            return opCode.ToString().StartsWith("CMSG") ? PacketHeaderType.WorldCmsg : PacketHeaderType.WorldSmsg;
        }

        public static PacketHeaderType Parse(this AuthServerOpCode opCode)
        {
            return PacketHeaderType.AuthSmsg;
        }
    }
}