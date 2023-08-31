using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using MIIDIToLFO.Lib.Convert.LFO;

namespace MIIDIToLFO.Lib.Convert
{
    public class MidiConvert
    {
        public static MidiConvertResult ConvertMidiAndSave(string? path, ConversionTarget target, Config config)
        {
            var outPath = "";
            var extension = "";

            switch (target)
            {
                case ConversionTarget.Serum:
                    outPath = Config.instance.serumFolderPath;
                    extension = "shp";
                    break;
                case ConversionTarget.Vital:
                    outPath = Config.instance.vitalFolderPath;
                    extension = "vitallfo";
                    break;
            }

            var result = ConvertMidi(path, target, config);

            WriteShapeFiles(path, outPath, extension, result);

            Printer.Print("Done.");

            return result;
        }

        public static MidiConvertResult ConvertMidi(string? path, ConversionTarget target, Config config)
        {
            switch (target)
            {
                case ConversionTarget.Serum:
                default:
                    return ConvertMidiSerum(path, config);
                case ConversionTarget.Vital:
                    return ConvertMidiVital(path,config);
            }
        }

        public static MidiConvertResult ConvertMidiSerum(string? path, Config config)
        {
            var lfoShapePitch = new SerumLFOShape();
            var lfoShapeGate = new SerumLFOShape();

            return ConvertInternal(lfoShapePitch, lfoShapeGate, path, config);
        }

        public static MidiConvertResult ConvertMidiVital(string? path, Config config)
        {
            var name = "";
            var _name = Path.GetFileNameWithoutExtension(path);
            if (_name != null )
                name = _name;

            var lfoShapePitch = new VitalLFOShape(name + " (Pitch)");
            var lfoShapeGate = new VitalLFOShape(name + " (Gate)");

            return ConvertInternal(lfoShapePitch, lfoShapeGate, path, config);
        }

        private static MidiConvertResult ConvertInternal(LFOShape lfoShapePitch, LFOShape lfoShapeGate, string? path, Config config)
        {
            MidiInfo info = new MidiInfo(path, config);

            PrintMidiInfo(info);

            // List of relative positions for conversion.
            var notes = new List<MidiNoteInfo>();
            foreach (var note in info.midiFile.GetNotes())
                notes.Add(new MidiNoteInfo(
                    note,
                    info.ticks,
                    info.ticksGate,
                    config.absolutePitch ? info.noteRangeAbs : info.noteRange,
                    config.absolutePitch ? info.minNote.NoteNumber : info.minC.NoteNumber,
                    config.readVelocity));

            if (notes.Count > 0)
            {
                // Gate calculation.
                if (!config.ignoreGate)
                    ConvertGate(lfoShapeGate, notes, config);

                // Pitch calculation.
                if (!config.ignorePitch)
                    ConvertPitch(lfoShapePitch, notes, config);
            }

            PrintShapeInfo(lfoShapePitch, lfoShapeGate);

            return new MidiConvertResult(lfoShapePitch, lfoShapeGate, info);
        }

        public static void ConvertGate(LFOShape lfoShapeGate, List<MidiNoteInfo> notes, Config config)
        {
            var firstNote = notes[0];

            // Determine velocity for the first point at x 0.0.
            var lastNote = firstNote;
            double firstVelocity = firstNote.fractionStartGate == 0.0 ? firstNote.velocity : 1.0;
            lfoShapeGate.AddPoint(new LFOPoint(0.0, firstVelocity));

            for (int i = 0; i < notes.Count; i++)
            {
                var note = notes[i];

                // Relative start and end times of the note between 0.0 and 1.0.
                var fractionStart = note.fractionStartGate;
                var fractionEnd = note.fractionEndGate;

                if (fractionStart > 1.0 || fractionEnd > 1.0)
                    break;

                if (config.optimizeShapes)
                {
                    // Loop through following notes and extend the current section until the effective velocity changes.
                    while (i + 1 < notes.Count)
                    {
                        var nextNote = notes[i + 1];
                        if (fractionEnd == nextNote.fractionStartGate && note.velocity == nextNote.velocity && nextNote.fractionStartGate <= 1.0 && nextNote.fractionEndGate <= 1.0)
                        {
                            fractionEnd = nextNote.fractionEndGate;
                            i++;
                        }
                        else
                            break;
                    }
                }

                // Gate square start.
                // We've already set the initial value at 0.0, let's skip it here.
                if (fractionStart > 0.0)
                {
                    // Don't do the full square for consecutive notes.
                    if (config.optimizeShapes)
                    {
                        if (fractionStart != lastNote.fractionEnd)
                            lfoShapeGate.AddPoint(new LFOPoint(fractionStart, 1.0));
                    }
                    else
                        lfoShapeGate.AddPoint(new LFOPoint(fractionEnd, 1.0));

                    lfoShapeGate.AddPoint(new LFOPoint(fractionStart, note.velocity));
                }

                // Gate square end.
                // 1.0 position needs different handling, let's skip it here.
                if (fractionEnd < 1.0)
                {
                    lfoShapeGate.AddPoint(new LFOPoint(fractionEnd, note.velocity));

                    // Don't do the full square for consecutive notes.
                    if (config.optimizeShapes)
                    {
                        var nextNote = notes[Math.Min(notes.Count - 1, i + 1)];
                        if (fractionEnd != nextNote.fractionStartGate)
                            lfoShapeGate.AddPoint(new LFOPoint(fractionEnd, 1.0));
                    }
                    else
                        lfoShapeGate.AddPoint(new LFOPoint(fractionEnd, 1.0));
                }

                lastNote = note;
            }

            // Determine velocity at the 1.0 position and set if different than the loopback point.
            double lastVelocity = lastNote.fractionEndGate >= 1.0 ? lastNote.velocity : 1.0;
            if (firstVelocity != lastVelocity)
                lfoShapeGate.AddPoint(new LFOPoint(1.0, lastVelocity));

            // Default loopback point, same value as the first point. Needed for seamless looping. Should be invisible for Serum.
            lfoShapeGate.AddPoint(new LFOPoint(1.0, firstVelocity));
        }

        public static void ConvertPitch(LFOShape lfoShapePitch, List<MidiNoteInfo> notes, Config config)
        {
            var firstNote = notes[0];

            // Determine pitch for first point at x 0.0.
            lfoShapePitch.AddPoint(new LFOPoint(0.0, firstNote.pitch));

            double lastPitch = firstNote.pitch;

            for (int i = 0; i < notes.Count; i++)
            {
                var note = notes[i];

                if (config.optimizeShapes)
                {
                    // Loop through the following notes and skip until the effective pitch changes.
                    while (i + 1 < notes.Count)
                    {
                        var nextNote = notes[i + 1];
                        if (nextNote.pitch == note.pitch)
                            i++;
                        else
                            break;
                    }
                }

                // We've already set the initial value at 0.0, let's skip it here.
                if (note.fractionStart > 0.0)
                {
                    lfoShapePitch.AddPoint(new LFOPoint(note.fractionStart, lastPitch));
                    lfoShapePitch.AddPoint(new LFOPoint(note.fractionStart, note.pitch));
                }

                lastPitch = note.pitch;
            }

            // Add another point at the end if the pitch is different than the loopback point.
            if (firstNote.pitch != lastPitch)
                lfoShapePitch.AddPoint(new LFOPoint(1.0, lastPitch));

            // Default loopback point, same value as the first point. Needed for seamless looping. Should be invisible for Serum.
            lfoShapePitch.AddPoint(new LFOPoint(1.0, firstNote.pitch));
        }

        public static void WriteShapeFiles(string? inPath, string? outPath, string fileExtension, MidiConvertResult result)
        {
            if (!Directory.Exists(outPath))
            {
                Printer.Print("Output directory not set or doesn't exist, falling back to input directory.");
                outPath = Path.GetPathRoot(inPath);
            }

            if (Directory.Exists(outPath))
            {
                var fileName = Path.GetFileNameWithoutExtension(inPath);
                var noteRange = Config.instance.absolutePitch ? result.info.noteRangeAbs : result.info.noteRange;

                if (!Config.instance.ignorePitch)
                {
                    var fileNamePitch = Path.Combine(outPath, fileName + " (" + result.info.bars + " Bar, " + noteRange + "st) (Pitch)." + fileExtension);
                    File.WriteAllBytes(fileNamePitch, result.lfoShapePitch.ToBinary());
                }

                if (!Config.instance.ignoreGate)
                {
                    var fileNameGate = Path.Combine(outPath, fileName + " (" + result.info.barsGate + " Bar, " + noteRange + "st) (Gate)." + fileExtension);
                    File.WriteAllBytes(fileNameGate, result.lfoShapeGate.ToBinary());
                }
            }

            else
                Printer.Print("Couldn't write files, directory doesn't exist.");
        }

        public static void PrintMidiInfo(MidiInfo info)
        {
            Printer.Print("Length: " + info.bars + "," + info.barsGate + " (" + info.barsFrac + ") Bars, " + info.ticks + " Ticks");
            Printer.Print("Min Note: " + info.minNote.NoteName + info.minNote.Octave);
            Printer.Print("Max Note: " + info.maxNote.NoteName + info.maxNote.Octave);
            Printer.Print("Octave Range: " + info.octaveRange + " (" + 12 * info.octaveRange + "st)");
            Printer.Print("Note Range: " + info.noteRangeAbs + "st");
        }

        public static void PrintShapeInfo(LFOShape lfoShapePitch, LFOShape lfoShapeGate)
        {
            if (lfoShapePitch.index < lfoShapePitch.maxNumPoints)
                Printer.Print("Pitch shape: " + lfoShapePitch.index + " points");
            else
                Printer.Print("Pitch shape might be truncated (max number of points (" + lfoShapePitch.maxNumPoints + ") reached).");

            if (lfoShapeGate.index < lfoShapeGate.maxNumPoints)
                Printer.Print("Gate shape: " + lfoShapeGate.index + " points");
            else
                Printer.Print("Gate shape might be truncated (max number of points (" + lfoShapeGate.maxNumPoints + ") reached).");
        }
    }
}