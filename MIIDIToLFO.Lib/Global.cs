using System;
using System.Diagnostics;
using System.IO;

namespace MIIDIToLFO.Lib
{
    public static class Global
    {
        public const string version = "1.0.0";

        public static string GetVersionString()
        {
            return "MIIDIToLFO.Lib " + version;
        }
        public static string GetAppDataDirPath()
        {
            string[] paths = { Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OxyDSP", "MIIDIToLFO" };
            return Path.Combine(paths);
        }

        public static void OpenPathInExplorer(string? dir)
        {
            var result = Directory.Exists(dir);
            if (result)
                result = OpenPathInShell(dir);

            if (!result)
                Printer.Print("Folder not set or doesn't exist.");
        }

        // Should work on most platforms.
        public static bool OpenPathInShell(string? dir)
        {
            var result = Uri.TryCreate(dir, UriKind.Absolute, out Uri? uri);
            if (result && uri != null)
            {
                var p = new Process();
                p.StartInfo.FileName = uri.ToString();
                p.StartInfo.UseShellExecute = true;
                p.Start();
            }

            return result;
        }
    }
}
