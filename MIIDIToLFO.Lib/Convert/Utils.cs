using Melanchall.DryWetMidi.Common;

namespace MIIDIToLFO.Lib.Convert
{
    public class Utils
    {
        public static double GetVelocity(SevenBitNumber velocity)
        {
            return 1.0 - ((double)velocity / (double)0x7F);
        }
        public static double GetRelativePitch(SevenBitNumber note, SevenBitNumber minNote, int noteRange)
        {
            return 1.0 - ((double)(note - minNote) / (double)noteRange);
        }

        public static double Map0To1(double src, double dstMin, double dstMax)
        {
            return dstMin + src * (dstMax - dstMin);
        }

        // Should be good enough for dealing with bar lengths.
        public static uint NextPow2(uint v)
        {
            v--;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            v++;

            return v;
        }
    }
}
