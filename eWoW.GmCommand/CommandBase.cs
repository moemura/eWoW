namespace eWoW.GmCommand
{
    public enum CommandTriggerStart
    {
        Slash,
        Period,
        Both
    }

    public abstract class CommandBase
    {
        public abstract string Trigger { get; }
        public abstract string HelpText { get; }

        public abstract CommandTriggerStart StartRequirement { get; }

        public abstract bool HandleCommand(params string[] args);
    }
}