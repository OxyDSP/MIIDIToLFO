using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using MIIDIToLFO.Lib;
using System.Linq;
using System.IO;
using System;
using MIIDIToLFO.Lib.Convert;
using System.Diagnostics;

namespace MIIDIToLFO.Avalonia.Views;

public partial class MainView : UserControl
{
    public static MainView? instance { get; private set; }

    public MainView()
    {
        instance = this;
        InitializeComponent();
        outputTextBox.Clear();
        versionLabel.Content = Global.GetVersionString();
        Printer.SetOnPrint(Print);
        Config.SetOnConfigLoad(OnConfigLoad);
        Config.Load();
    }

    public static void Print(string message)
    {
        if (instance != null)
        {
            instance.outputTextBox.Text += message + Environment.NewLine;
        }
    }

    public static void OnConfigLoad()
    {
        if (instance != null)
        {
            instance.readVelocityCheckbox.IsChecked = Config.instance.readVelocity;
            instance.ignoreGateCheckbox.IsChecked = Config.instance.ignoreGate;
            instance.ignorePitchCheckbox.IsChecked = Config.instance.ignorePitch;
            instance.absolutePitchCheckbox.IsChecked = Config.instance.absolutePitch;
            instance.optimizeShapesCheckbox.IsChecked = Config.instance.optimizeShapes;
            instance.lengthPow2Checkbox.IsChecked = Config.instance.lengthPow2;
            instance.gateTruncateComboBox.SelectedIndex = (int)Config.instance.gateTruncate;
        }
    }

    public static void OnDragDrop(string path)
    {
        if (instance != null)
        {
            instance.SetInputMIDIPath(path);
        }
    }

    public void SetInputMIDIPath(string? path)
    {
        if (path != null)
        {
            fileNameTextBox.Text = path;
            Config.instance.lastInputMidiPath = Path.GetDirectoryName(path);
            Config.Save();
        }
    }

    private async void OnSetInputMIDI(object sender, RoutedEventArgs args)
    {
        if (MainWindow.instance != null)
        {
            var result = await MainWindow.instance.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select MIDI",
                AllowMultiple = false,
                SuggestedStartLocation = Directory.Exists(Config.instance.lastInputMidiPath) ?
                await MainWindow.instance.StorageProvider.TryGetFolderFromPathAsync(Config.instance.lastInputMidiPath) :
                await MainWindow.instance.StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Documents)
            });

            if (result.Count >= 1)
                SetInputMIDIPath(result.First().Path.LocalPath);
        }
    }

    private async void OnSetSerumFolder(object sender, RoutedEventArgs args)
    {
        if (MainWindow.instance != null)
        {
            var result = await MainWindow.instance.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select Serum Folder",
                AllowMultiple = false,
                SuggestedStartLocation = Directory.Exists(Config.instance.serumFolderPath) ? 
                await MainWindow.instance.StorageProvider.TryGetFolderFromPathAsync(Config.instance.serumFolderPath) : 
                await MainWindow.instance.StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Documents)
            });

            if (result.Count >= 1)
            {
                Config.instance.serumFolderPath = result.First().Path.LocalPath;
                Config.Save();
            }
        }
    }

    private async void OnSetVitalFolder(object sender, RoutedEventArgs args)
    {
        if (MainWindow.instance != null)
        {
            var result = await MainWindow.instance.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select Vital Folder",
                AllowMultiple = false,
                SuggestedStartLocation = Directory.Exists(Config.instance.vitalFolderPath) ?
                await MainWindow.instance.StorageProvider.TryGetFolderFromPathAsync(Config.instance.vitalFolderPath) :
                await MainWindow.instance.StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Documents)
            });

            if (result.Count >= 1)
            {
                Config.instance.vitalFolderPath = result.First().Path.LocalPath;
                Config.Save();
            }
        }
    }

    private void OnShowSerumFolder(object sender, RoutedEventArgs args)
    {
        Global.OpenPathInExplorer(Config.instance.serumFolderPath);
    }

    private void OnShowVitalFolder(object sender, RoutedEventArgs args)
    {
        Global.OpenPathInExplorer(Config.instance.vitalFolderPath);
    }

    private void OnShowDataFolder(object sender, RoutedEventArgs args)
    {
        Global.OpenPathInExplorer(Global.GetAppDataDirPath());
    }

    private void OnLogoClick(object sender, RoutedEventArgs args)
    {
        Global.OpenPathInShell("https://oxydsp.com/");
    }

    private void OnReadVelocityClicked(object sender, RoutedEventArgs args)
    {
        if (readVelocityCheckbox.IsChecked != null)
        {
            Config.instance.readVelocity = (bool)readVelocityCheckbox.IsChecked;
            Config.Save();
        }
    }

    private void OnIgnoreGateClicked(object sender, RoutedEventArgs args)
    {
        if (ignoreGateCheckbox.IsChecked != null)
        {
            Config.instance.ignoreGate = (bool)ignoreGateCheckbox.IsChecked;
            Config.Save();
        }
    }

    private void OnIgnorePitchClicked(object sender, RoutedEventArgs args)
    {
        if (ignorePitchCheckbox.IsChecked != null)
        {
            Config.instance.ignorePitch = (bool)ignorePitchCheckbox.IsChecked;
            Config.Save();
        }
    }

    private void OnOptimizeShapesClicked(object sender, RoutedEventArgs args)
    {
        if (optimizeShapesCheckbox.IsChecked != null)
        {
            Config.instance.optimizeShapes = (bool)optimizeShapesCheckbox.IsChecked;
            Config.Save();
        }
    }

    private void OnAbsolutePitchClicked(object sender, RoutedEventArgs args)
    {
        if (absolutePitchCheckbox.IsChecked != null)
        {
            Config.instance.absolutePitch = (bool)absolutePitchCheckbox.IsChecked;
            Config.Save();
        }
    }

    private void OnLengthPow2Clicked(object sender, RoutedEventArgs args)
    {
        if (lengthPow2Checkbox.IsChecked != null)
        {
            Config.instance.lengthPow2 = (bool)lengthPow2Checkbox.IsChecked;
            Config.Save();
        }
    }

    private void OnGateTruncateSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (gateTruncateComboBox != null)
        {
            Config.instance.gateTruncate = (GateTruncateOption)gateTruncateComboBox.SelectedIndex;
            Config.Save();
        }
    }

    private void OnConvertSerum(object sender, RoutedEventArgs args)
    {
        OnConvert(ConversionTarget.Serum);
    }

    private void OnConvertVital(object sender, RoutedEventArgs args)
    {
        OnConvert(ConversionTarget.Vital);
    }

    private void OnConvert(ConversionTarget target)
    {
        try
        {
            outputTextBox.Clear();
            MidiConvert.ConvertMidiAndSave(fileNameTextBox.Text, target, Config.instance);
        }

        catch (Exception ex)
        {
            Print(ex.Message);
        }
    }
}
