using System;

namespace SourceAFIS.Dummy
{
    public class IPAddress
    {
        public static short HostToNetworkOrder(short number)
        {
            if (BitConverter.IsLittleEndian)
            {
                uint unsigned = (uint)(ushort)number;
                return (short)(ushort)((unsigned >> 8) | (unsigned << 8));
            }
            else
                return number;
        }

        public static short NetworkToHostOrder(short number)
        {
            return HostToNetworkOrder(number);
        }

        public static int HostToNetworkOrder(int number)
        {
            if (BitConverter.IsLittleEndian)
            {
                uint unsigned = (uint)number;
                return (int)((unsigned >> 24) | ((unsigned >> 8) & 0xff00) | ((unsigned << 8) & 0xff0000) | (unsigned << 24));
            }
            else
                return number;
        }

        public static int NetworkToHostOrder(int number)
        {
            return HostToNetworkOrder(number);
        }
    }
}
