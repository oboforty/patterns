using System;
using System.Collections;
using System.Text;

namespace QServerSDK
{
    public class Anyad
    {
        public static string BitString(int val)
        {
            return BitString(BitConverter.GetBytes(val));
        }
        public static string BitString(uint val)
        {
            return BitString(BitConverter.GetBytes(val));
        }

        public static string BitString(byte val)
        {
            return BitString(new byte[]{ val });
        }
        public static string BitString(short val)
        {
            return BitString(BitConverter.GetBytes(val));
        }
        public static string BitString(ushort val)
        {
            return BitString(BitConverter.GetBytes(val));
        }

        public static string BitString(byte[] byts)
        {
            // BitiString representation: bits grow from right to left

            var sb = new StringBuilder();

            for (int i = byts.Length-1; i >= 0; i--)
            {
                for (int b = 0; b < 8; b++)
                    sb.Append((byts[i] & (1 << (7-b))) > 0 ? '1' : '0');
                sb.Append(' ');
            }

            return sb.ToString().Trim();
        }

    }
}
