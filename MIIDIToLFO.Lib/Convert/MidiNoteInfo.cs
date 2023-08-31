using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Common;

namespace MIIDIToLFO.Lib.Convert
{
    public class MidiNoteInfo
    {
        public Note? note;
        public double fractionStart;
        public double fractionEnd;
        public double fractionStartGate;
        public double fractionEndGate;
        public double pitch;
        public double velocity;

        public MidiNoteInfo(Note? note, double ticks, double ticksGate, int noteRange, SevenBitNumber minNote, bool readVelocity)
        {
            this.note = note;
            if (note != null )
            {
                fractionStart = note.Time / ticks;
                fractionEnd = note.EndTime / ticks;
                fractionStartGate = note.Time / ticksGate;
                fractionEndGate = note.EndTime / ticksGate;
                pitch = Utils.GetRelativePitch(note.NoteNumber, minNote, noteRange);
                velocity = readVelocity ? Utils.GetVelocity(note.Velocity) : 0.0;
            }
        }
    }
}
