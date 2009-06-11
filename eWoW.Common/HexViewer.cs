using System;

namespace eWoW.Common
{
    public static class HexViewer
    {
        public static string GetHexString(byte[] bytes)
        {
            string tmp = string.Empty;
            foreach (byte b in bytes)
            {
                tmp += b.ToString("X2") + " ";
            }
            return tmp;
        }

        public static string GetHexStringFormatted(byte[] bytes, int lengthPerLine)
        {
            string ret = string.Empty;
            string lineCharacters = string.Empty;
            int byteCounter = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                byteCounter++;
                // Make sure we keep the first line... on the first line
                if (i != 0 && i % lengthPerLine == 0)
                {
                    ret += " " + lineCharacters + Environment.NewLine;
                    lineCharacters = "";
                    byteCounter = 0;
                }

                ret += bytes[i].ToString("X2") + " ";

                if (bytes[i] > 31 && bytes[i] < 128)
                {
                    lineCharacters += (char) bytes[i];
                }
                else
                {
                    lineCharacters += ".";
                }

                // Last byte, make sure we wrote our line.
                if (i == (bytes.Length - 1) && byteCounter != 0)
                {
                    if (!string.IsNullOrEmpty(lineCharacters))
                    {
                        int bytesRemainingInLine = byteCounter - i;
                        var s = new string(' ', bytesRemainingInLine * 2);
                        Console.WriteLine(bytesRemainingInLine + " bytes remaning");
                        ret += s;
                        ret += lineCharacters;
                    }
                }
            }
            return ret;
        }

        public static void View(byte[] b, int offset, int length)
        {
            View(b, offset, length, true);
        }

        public static void View(byte[] b, int offset, int len, bool noChar)
        {
            if (noChar)
            {
                for (int t = 0; t < len; t += 32)
                {
                    for (int i = t; i < t + 32 && i + offset < b.Length; i++)
                    {
                        if (i < len)
                        {
                            Console.Write("0x{0}, ", b[i + offset].ToString("X2"));
                        }
                        else
                        {
                            Console.Write(" | ");
                        }
                    }
                    Console.WriteLine("");
                }
            }
            else
            {
                for (int t = 0; t < len; t += 16)
                {
                    for (int i = t; i < t + 16 && i + offset < b.Length; i++)
                    {
                        if (i < len)
                        {
                            Console.Write("{0} ", b[i + offset].ToString("X2"));
                        }
                        else
                        {
                            Console.Write(" | ");
                        }
                    }
                    for (int i = t; i + offset < b.Length && i < t + 16 && i < len; i++)
                    {
                        if (b[i + offset] > 31 && b[i + offset] < 128)
                        {
                            Console.Write((char) b[i + offset]);
                        }
                        else
                        {
                            Console.Write(".");
                        }
                    }
                    Console.WriteLine("");
                }
            }
        }
    }
}