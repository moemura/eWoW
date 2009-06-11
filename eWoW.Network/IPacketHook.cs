using System;
using System.Collections.Generic;

namespace eWoW.Network
{
    public enum PacketRespondType
    {
        In,
        Out,
        Both
    }

    public interface IPacketHook
    {
        PacketRespondType RespondType { get; }
        List<uint> AcceptedOpCodes { get; }

        void HandlePacket(ClientConnection connection, Packet p);
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    internal sealed class SkipPacketHookAttribute : Attribute
    {}
}