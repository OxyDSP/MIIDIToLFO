using System.Text.Json;
using MIIDIToLFO.Lib.Convert;

namespace MIIDIToLFO.Lib
{
    public class Config
    {
        public static Config instance = new Config();
        public bool readVelocity { get; set; } = false;
        public bool ignoreGate { get; set; } = false;
        public bool ignorePitch { get; set; } = false;
        public bool absolutePitch { get; set; } = false;
        public bool optimizeShapes { get; set; } = true;
        public bool lengthPow2 { get; set; } = true;
        public GateTruncateOption gateTruncate { get; set; } = GateTruncateOption.Off;
        public string? serumFolderPath { get; set; } = "";
        public string? vitalFolderPath { get; set; } = "";
        public string? lastInputMidiPath { get; set; } = "";

        private static Action? OnConfigLoad;

        public static void SetOnConfigLoad(Action onConfigLoad)
        {
            OnConfigLoad = onConfigLoad;
        }

        public static void Load()
        {
            try
            {
                var dir = Global.GetAppDataDirPath();
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var configPath = Path.Combine(dir, "config.json");
                if (File.Exists(configPath))
                {
                    var configJson = File.ReadAllText(configPath);
                    var obj = JsonSerializer.Deserialize(configJson, typeof(Config));
                    if (obj != null)
                    {
                        instance = (Config)obj;
                        OnConfigLoad?.Invoke();
                    }
                }

                else
                    Save();
            }

            catch (Exception ex)
            {
                Printer.Print(ex.Message);
            }
        }

        public static void Save()
        {
            try
            {
                var json = JsonSerializer.Serialize(instance);

                var dir = Global.GetAppDataDirPath();
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var configPath = Path.Combine(dir, "config.json");
                File.WriteAllText(configPath, json);
            }

            catch (Exception ex)
            {
                Printer.Print(ex.Message);
            }
        }
    }
}
