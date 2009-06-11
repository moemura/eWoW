using System;

namespace eWoW.Common
{
    public static class Events
    {
        public static event EventHandler ServerExited;

        internal static void InvokeServerExited(EventArgs e)
        {
            EventHandler exited = ServerExited;
            if (exited != null)
            {
                exited(typeof(Events), e);
            }
        }
    }
}