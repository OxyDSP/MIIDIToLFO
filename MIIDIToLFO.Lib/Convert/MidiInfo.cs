using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;

namespace MIIDIToLFO.Lib.Convert
{
    public class MidiInfo
    {
        public MidiFile midiFile;
        public TempoMap tempoMap;
        public double barsFrac;
        public double bars;
        public double ticks;
        public double barsGate;
        public double ticksGate;
        public Melanchall.DryWetMidi.Interaction.Note minNote;
        public Melanchall.DryWetMidi.Interaction.Note maxNote;
        public Melanchall.DryWetMidi.MusicTheory.Note minC;
        public Melanchall.DryWetMidi.MusicTheory.Note maxC;
        public int minOctave;
        public int maxOctave;
        public int octaveRange;
        public int noteRange;
        public int noteRangeAbs;

        // Extract relevant info from the MIDI file in a more usable format.
        public MidiInfo(string? path, Config config)
        {
            if (path == null)
                throw new ArgumentNullException("path was null.");

            var _midiFile = MidiFile.Read(path);
            if (_midiFile == null)
                throw new ApplicationException("midiFile was null.");
            midiFile = _midiFile;

            var _tempoMap = midiFile.GetTempoMap();
            if (_tempoMap == null)
                throw new ApplicationException("tempoMap was null.");
            tempoMap = _tempoMap;

            // Convert duration string in the format "x/y" to fractional bar length.
            var durationStr = midiFile.GetDuration(TimeSpanType.Musical).ToString();
            if (durationStr == null)
                throw new ApplicationException("Duration was null.");

            var split = durationStr.Split('/');
            if (split.Length != 2)
                throw new ApplicationException("Bad duration format: " + durationStr);

            barsFrac = double.Parse(split[0]) / double.Parse(split[1]);

            // Round up the bar length to the next whole number or power of 2 to better fit the available timespans in the LFOs.
            bars = config.lengthPow2 ? Utils.NextPow2((uint)Math.Ceiling(barsFrac)) : Math.Ceiling(barsFrac);
            ticks = TimeConverter.ConvertFrom(MusicalTimeSpan.FromDouble(bars), tempoMap);

            // Shorten gate length if needed.
            barsGate = config.gateTruncate == GateTruncateOption.Off ? bars : 1 << ((int)config.gateTruncate - 1);
            if (barsGate > bars)
                barsGate = bars;
            ticksGate = TimeConverter.ConvertFrom(MusicalTimeSpan.FromDouble(barsGate), tempoMap);

            // Determine note ranges.
            var _minNote = midiFile.GetNotes().MinBy(note => note.NoteNumber);
            if (_minNote == null)
                throw new ApplicationException("minNote was null.");
            minNote = _minNote;

            var _maxNote = midiFile.GetNotes().MaxBy(note => note.NoteNumber);
            if (_maxNote == null)
                throw new ApplicationException("maxNote was null.");
            maxNote = _maxNote;

            minOctave = minNote.Octave;
            maxOctave = maxNote.Octave + 1;
            octaveRange = maxOctave - minNote.Octave;

            var _minC = Octave.Get(minNote.Octave).C;
            if (_minC == null)
                throw new ApplicationException("minC was null.");
            minC = _minC;

            var _maxC = Octave.Get(maxOctave).C;
            if (_maxC == null)
                throw new ApplicationException("maxC was null.");
            maxC = _maxC;

            noteRange = maxC.NoteNumber - minC.NoteNumber;
            noteRangeAbs = maxNote.NoteNumber - minNote.NoteNumber;
        }
    }
}
