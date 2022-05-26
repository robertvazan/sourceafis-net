// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net

namespace SourceAFIS.Primitives
{
    static class Integers
    {
        public static int Sq(int value) { return value * value; }
        public static int RoundUpDiv(int dividend, int divisor) { return (dividend + divisor - 1) / divisor; }
        // https://stackoverflow.com/questions/10439242/count-leading-zeroes-in-an-int32
        // Modified for unsigned values.
        // .NET Core 3 has BitOperations.PopCount() and BitOperations.LeadingZeroCount().
        public static uint PopulationCount(uint x)
        {
            x -= x >> 1 & 0x55555555;
            x = (x >> 2 & 0x33333333) + (x & 0x33333333);
            x = (x >> 4) + x & 0x0f0f0f0f;
            x += x >> 8;
            x += x >> 16;
            return x & 0x0000003f;
        }
        public static uint LeadingZeros(uint x)
        {
            //compile time constant
            const uint numIntBits = sizeof(int) * 8;
            //do the smearing
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            //count the ones
            x -= x >> 1 & 0x55555555;
            x = (x >> 2 & 0x33333333) + (x & 0x33333333);
            x = (x >> 4) + x & 0x0f0f0f0f;
            x += x >> 8;
            x += x >> 16;
            return numIntBits - (x & 0x0000003f); //subtract # of 1s from 32
        }
    }
}
