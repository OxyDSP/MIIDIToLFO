using MIIDIToLFO.Lib.Convert.LFO;

namespace MIIDIToLFO.Lib.Convert
{
    public class MidiConvertResult
    {
        public LFOShape lfoShapePitch;
        public LFOShape lfoShapeGate;
        public MidiInfo info;

        public MidiConvertResult(LFOShape lfoShapePitch, LFOShape lfoShapeGate, MidiInfo info)
        {
            this.lfoShapePitch = lfoShapePitch;
            this.lfoShapeGate = lfoShapeGate;
            this.info = info;
        }
    }
}
