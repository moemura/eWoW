using System;
using System.Collections.Generic;
using eWoW.Common.Collections;
using eWoW.Network;
using eWoW.Network.OpCodes;

namespace eWoW.GmCommand
{
    public class CommandHandler : IPacketHook
    {
        private const PacketRespondType _respondType = PacketRespondType.In;
        private readonly List<uint> _acceptedOpCodes = new List<uint> {(uint) RealmServerOpCode.SMSG_MESSAGECHAT};
        private readonly ClassCollection<CommandBase> _commands = new ClassCollection<CommandBase>();

        public CommandHandler()
        {
            // Since this class will automagically be created for us, thanks to the PacketHandler class
            // we can just do our normal ctor logic here. In this case, we'll load up any commands.

            Commands.LoadClassesInFolder("/"); // Current folder. Relative path!
        }

        public ClassCollection<CommandBase> Commands { get { return _commands; } }

        #region IPacketHook Members

        public PacketRespondType RespondType { get { return _respondType; } }

        public List<uint> AcceptedOpCodes { get { return _acceptedOpCodes; } }

        public void HandlePacket(ClientConnection connection, Packet p)
        {
            // TODO: Use the eWoW.Chat system to retrieve the string from this packet.
            string chatString = "";
            foreach (CommandBase cmd in Commands)
            {
                bool canRun;
                switch (cmd.StartRequirement)
                {
                    case CommandTriggerStart.Slash:
                        canRun = chatString.StartsWith("/" + cmd.Trigger);
                        break;
                    case CommandTriggerStart.Period:
                        canRun = chatString.StartsWith("." + cmd.Trigger);
                        break;
                    case CommandTriggerStart.Both:
                        canRun = chatString.StartsWith("/" + cmd.Trigger) || chatString.StartsWith("." + cmd.Trigger);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if (canRun)
                {
                    if (!cmd.HandleCommand(chatString.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)))
                    {
                        PrintHelpText(connection, p, cmd.HelpText);
                    }
                }
            }
        }

        #endregion

        private void PrintHelpText(ClientConnection conn, Packet p, string text)
        {
            // TODO: Use the chat system... again... to send a system message about the command help text.
        }
    }
}