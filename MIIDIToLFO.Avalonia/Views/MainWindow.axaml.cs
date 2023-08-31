using Avalonia.Controls;
using Avalonia.Input;
using System.Linq;

namespace MIIDIToLFO.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        instance = this;
        AddHandler(DragDrop.DropEvent, (sender, args) => { OnDragDrop(sender, args); });
    }

    public static MainWindow? instance { get; private set; }

    private void OnDragDrop(object? sender, DragEventArgs args)
    {
        if (args.Data != null)
        {
            var files = args.Data.GetFiles();
            if (files != null)
            {
                var list = files.ToList();
                if (list.Count > 0)
                    MainView.OnDragDrop(list.First().Path.LocalPath);
            }
        }
    }
}
