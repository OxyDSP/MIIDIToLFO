using CommandLine;
using MIIDIToLFO.Lib.Convert;

namespace MIIDIToLFO.CLI
{
    public class Options
    {
        [Option('i', "input", Required = true, HelpText = "Input MIDI path.")]
        public string? InPath { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output LFO path.")]
        public string? OutPath { get; set; }

        [Option('t', "target", Required = true, HelpText = "Conversion target. 0 - Serum, 1 - Vital.")]
        public ConversionTarget Target { get; set; }

        [Option("ignore_gate", Default = false, Required = false, HelpText = "Don't output gate LFO file.")]
        public bool IgnoreGate { get; set; }

        [Option("ignore_pitch", Default = false, Required = false, HelpText = "Don't output pitch LFO file.")]
        public bool IgnorePitch { get; set; }

        [Option("read_velocity", Default = false, Required = false, HelpText = "Converts velocity from MIDI to gate shape.")]
        public bool ReadVelocity { get; set; }

        [Option("absolute_pitch", Default = false, Required = false, HelpText = "Maps pitch LFO 0-1 to the actual note range instead of whole octaves.")]
        public bool AbsolutePitch { get; set; }

        [Option("optimize_shapes", Default = true, Required = false, HelpText = "Produces less LFO points by skipping insertion if the value would stay the same.")]
        public bool OptimizeShapes { get; set; }

        [Option("length_pow2", Default = true, Required = false, HelpText = "Sets MIDI bar length to the next power of 2 instead of the next whole number.")]
        public bool LengthPow2 { get; set; }

        [Option("gate_truncate", Default = GateTruncateOption.Off, Required = false, HelpText = "Cuts gate LFO after the set length: 0 - Off, 1 - _1Bar, 2 - _2Bar, 3 - _4Bar, 4 - _8Bar, 5 - _16Bar.")]
        public GateTruncateOption GateTruncate { get; set; }
    }
}
