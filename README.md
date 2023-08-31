## MIIDIToLFO
MIIDIToLFO is a free .NET app for Windows, macOS and Linux to convert MIDI files to LFO shapes for the [Serum](https://xferrecords.com/products/serum/) and [Vital](https://vital.audio/) synthesizers.  

![MIIDIToLFO](https://github.com/OxyDSP/MIIDIToLFO/assets/143701449/5bc49df6-4e13-4bcc-84fe-4f729abcfd11)

The conversion creates both Pitch and Gate shapes to properly represent note lengths and optionally note velocities.  
This allows you to quickly create sequenced patches, which would otherwise be very tedious as both synths don't include a sequencer.

## Installation
- Install the latest [.NET 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/7.0/) version for your OS/architecture.  
- Download the [latest release](https://github.com/OxyDSP/MIIDIToLFO/releases/latest) for your OS/architecture.  

## Usage
- Set the output folders. You can use the user LFO shape folder of the respective synth.  
   - Xfer\Serum Presets\LFO Shapes\User for Serum.  
   - Vital\User\LFOs for Vital.
- Drag & Drop a MIDI file into the window or press 'Set Input MIDI'.
- Adjust the options if desired.
- Press 'Convert Serum' or 'Convert Vital'.
- In the synth, load both shapes into an LFO each. Set the LFOs to the correct bar length and use the Trigger mode.
- Map the pitch LFO either to the Coarse Pitch/Transpose of the oscillators or the global Master Tune/Voice Transpose.  
   - Enter the semitone range as amount in the matrix. In Serum you need to enter the value with 'st' at the end ('24st' instead of just '24').  
   - Use unipolar direction so C plays the original pitch.
- Map the gate LFO to the level of the oscillators. Use unipolar direction.

## Options
- Ignore Gate
  - No Gate shape gets created.
- Ignore Pitch
  - No Pitch shape gets created.
- Read Velocity
  - Note velocities will be included in the Gate shape.
- Absolute Pitch
  - Actual note range from the MIDI will be mapped to y 0-1 in the LFO instead of using the nearest octaves.
  - Pitch will be different in relation to the note played.
  - Recommended to leave off.
- Optimize Shapes
  - Conversion will avoid naively setting points if the effective value doesn't change.
  - Can help to reduce the point count below the limit.
  - Recommended to leave on.
- Length pow2
  - Sets MIDI bar length to the next power of 2 instead of the next whole number.
- Gate Truncate
  - Cuts off the Gate shape at the set length.
  - Can help to reduce the point count below the limit if a longer sequence has a repeating rhythm.

## Limitations
- The main limitation is the maximum number of LFO points of each synth. Points above this count will be truncated.  
   - 480 for Serum.  
   - 136 for Vital.
- Both synths have a maximum synced LFO time of 32 bars.
- MIDI bar lengths work best at powers of 2 (1, 2, 4, 8, 16, 32).
- For correct results, sequences should be a single voice, meaning no overlapping notes.

## Projects
- MIIDIToLFO.Lib
  - Main library doing the conversion.  
- MIIDIToLFO.CLI
  - Lightweight command line implementation.
- MIIDIToLFO.Avalonia
  - GUI implementation using Avalonia.  
- MIIDIToLFO.GUI
  - Avalonia executable wrapper.  

## Dependencies
- [DryWetMidi](https://github.com/melanchall/drywetmidi)
- [Avalonia](https://github.com/AvaloniaUI/Avalonia) (GUI)
- [CommandLineParser](https://github.com/commandlineparser/commandline) (CLI)
- [Dotnet.Bundle](https://github.com/egramtel/dotnet-bundle) (macOS app bundle creation)
