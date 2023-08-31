namespace MIIDIToLFO.Lib.Convert.LFO
{
    // Abstract class to ideally use the same code for most of the conversion steps.
    public abstract class LFOShape
    {
        public uint maxNumPoints = 0;
        public uint index = 0;
        public abstract void AddPoint(LFOPoint point);
        public abstract byte[] ToBinary();
    }
}
