using System;
using eWoW.Common;

namespace eWoW.LogonServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var rand = new Random();
            var bytes = new byte[rand.Next(50, 100)];
            rand.NextBytes(bytes);
            Console.WriteLine(HexViewer.GetHexStringFormatted(bytes, 16));
            Console.WriteLine("{0}{0}", Environment.NewLine);
            HexViewer.View(bytes, 0, bytes.Length, false);
            Console.ReadLine();
        }
    }
}