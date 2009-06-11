using System.Collections.Generic;
using System.IO;
using System.Linq;
using eWoW.Common;
using eWoW.Network.OpCodes;

namespace eWoW.Network
{
    /// <summary>
    /// This class can probably be made static. But we'll leave it instanced just in case
    /// we ever decide to extend and move towards multi-servers per process.
    /// </summary>
    public class PacketHandler
    {
        private static HookCollection _hooks;

        public static List<IPacketHook> Hooks { get { return _hooks; } }

        /// <summary>
        /// Loads the hooks.
        /// </summary>
        /// <param name="directory">The directory.</param>
        public static void LoadHooks(string directory)
        {
            if (_hooks == null)
            {
                _hooks = new HookCollection(directory);
                Logging.Write(_hooks.ToString());
                return;
            }

            // Note; the directory can be hard coded somewhere. I'm leaving it as a
            // param requirement for extendability later.
            foreach (string s in Directory.GetFiles(directory, "*.dll"))
            {
                _hooks.LoadDll(s);
            }
            Logging.Write(_hooks.ToString());
        }

        public static void RunOutgoing(ClientConnection c, Packet p)
        {
            var op = (RealmServerOpCode) p.OpCode;

            IEnumerable<IPacketHook> acceptedHooks = from hook in Hooks
                                                     where
                                                         (hook.RespondType == PacketRespondType.Out ||
                                                          hook.RespondType == PacketRespondType.Both) &&
                                                         hook.AcceptedOpCodes.Contains((uint) op)
                                                     select hook;

            foreach (IPacketHook hook in acceptedHooks)
            {
                hook.HandlePacket(c, p);
            }
        }

        public static void RunIncoming(ClientConnection c, Packet p)
        {
            var op = (RealmServerOpCode) p.OpCode;

            IEnumerable<IPacketHook> acceptedHooks = from hook in Hooks
                                                     where
                                                         (hook.RespondType == PacketRespondType.In ||
                                                          hook.RespondType == PacketRespondType.Both) &&
                                                         hook.AcceptedOpCodes.Contains((uint) op)
                                                     select hook;

            foreach (IPacketHook hook in acceptedHooks)
            {
                hook.HandlePacket(c, p);
            }
        }
    }
}