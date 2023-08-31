using CommandLine;
using MIIDIToLFO.CLI;
using MIIDIToLFO.Lib;
using MIIDIToLFO.Lib.Convert;

static void RunOptions(Options opts)
{
    Printer.SetOnPrint(Console.WriteLine);
    Config.instance.serumFolderPath = opts.OutPath;
    Config.instance.vitalFolderPath = opts.OutPath;
    Config.instance.ignoreGate = opts.IgnoreGate;
    Config.instance.ignorePitch = opts.IgnorePitch;
    Config.instance.readVelocity = opts.ReadVelocity;
    Config.instance.absolutePitch = opts.AbsolutePitch;
    Config.instance.optimizeShapes = opts.OptimizeShapes;
    Config.instance.lengthPow2 = opts.LengthPow2;
    Config.instance.gateTruncate = opts.GateTruncate;

    try
    {
        MidiConvert.ConvertMidiAndSave(opts.InPath, opts.Target, Config.instance);
    }

    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

Console.WriteLine(Global.GetVersionString());
Parser.Default.ParseArguments<Options>(args).WithParsed(RunOptions);
