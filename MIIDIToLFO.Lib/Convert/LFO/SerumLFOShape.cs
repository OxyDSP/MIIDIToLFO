using System.Text;

namespace MIIDIToLFO.Lib.Convert.LFO
{
    public class SerumLFOShape : LFOShape
    {
        public const uint maxNumPointsConst = 480;
        public LFOPoint[] points = new LFOPoint[maxNumPointsConst];

        public SerumLFOShape()
        {
            // Fill up the points array with default values.
            Array.Fill(points, new LFOPoint());
            maxNumPoints = maxNumPointsConst;
        }

        public override void AddPoint(LFOPoint point)
        {
            if (index < points.Length)
            {
                points[index] = point;
                index++;
            }
        }

        /* Serum's LFO structure goes like this:
        Header      0x020 Bytes
        Powers      0xF00 Bytes (sizeof(double) * maxNumPoints)
        X Coords    0xF00 Bytes (sizeof(double) * maxNumPoints)
        Y Coords    0xF00 Bytes (sizeof(double) * maxNumPoints)
        Footer      0x028 Bytes
        */
        public override byte[] ToBinary()
        {
            byte[] bin = new byte[0x2D48];
            Array.Clear(bin, 0, bin.Length);
            var header = Encoding.ASCII.GetBytes("SERUMSHP)\x5");
            Array.Copy(header, 0, bin, 0, header.Length);

            uint offsetF = 0x20;
            uint offsetX = offsetF + maxNumPointsConst * sizeof(double);
            uint offsetY = offsetX + maxNumPointsConst * sizeof(double);

            foreach (var point in points)
            {
                BitConverter.GetBytes(point.f).CopyTo(bin, offsetF);
                BitConverter.GetBytes(point.x).CopyTo(bin, offsetX);
                BitConverter.GetBytes(point.y).CopyTo(bin, offsetY);
                offsetF += sizeof(double);
                offsetX += sizeof(double);
                offsetY += sizeof(double);
            }

            byte[] footer = new byte[0x28];
            Array.Clear(footer, 0, footer.Length);

            // Reduce point count by 1 to make the last point the loopback point.
            var indexBytes = BitConverter.GetBytes(Math.Max(index - 1, 0));
            Array.Copy(indexBytes, 0, footer, 8, indexBytes.Length);

            // Unsure if these value are all needed, but most LFO files have them in common.
            footer[0] = 1;
            footer[4] = 1;
            footer[0xC] = 0xFF;
            footer[0xD] = 0xFF;
            footer[0xE] = 0xFF;
            footer[0xF] = 0xFF;
            footer[0x11] = 2;

            Array.Copy(footer, 0, bin, 0x2D20, footer.Length);
            return bin;
        }
    }
}
