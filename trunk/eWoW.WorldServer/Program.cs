using System;
using System.Text;
using eWoW.Common;

namespace eWoW.WorldServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "eWoW World Server";
            Console.OutputEncoding = Encoding.UTF8;
            Logging.OnWrite += Logging_OnWrite;
            Logging.OnDebug += Logging_OnWrite;
            Logging.Write(Config.GetValue<string>("MySqlConfig.xml", "LogonSettings", "Host"));
            Console.ReadLine();
        }

        private static void Logging_OnWrite(string message, ConsoleColor col)
        {
            Console.ForegroundColor = col;
            Console.WriteLine(message);
        }
    }
}