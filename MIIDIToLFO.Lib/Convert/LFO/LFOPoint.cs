namespace MIIDIToLFO.Lib.Convert.LFO
{
    public class LFOPoint
    {
        public double x = 1.0;
        public double y = 1.0;
        public double f = 0.5;

        public LFOPoint()
        {

        }

        public LFOPoint(double x, double y, double f = 0.5)
        {
            this.x = x;
            this.y = y;
            this.f = f;
        }
    }
}
