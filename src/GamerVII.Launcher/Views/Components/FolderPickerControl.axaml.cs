using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Platform.Storage;
using ReactiveUI;

namespace GamerVII.Launcher.Views.Components;

public class FolderPickerControl : TemplatedControl
{
    public static readonly StyledProperty<ICommand> SelectFolderCommandProperty =
        AvaloniaProperty.Register<FolderPickerControl, ICommand>(
            nameof(SelectFolderCommand));

    public static readonly StyledProperty<string> SelectedFolderPathProperty =
        AvaloniaProperty.Register<FolderPickerControl, string>(
            nameof(SelectedFolderPath));

    public string SelectedFolderPath
    {
        get => GetValue(SelectedFolderPathProperty);
        set => SetValue(SelectedFolderPathProperty, value);
    }
    public ICommand SelectFolderCommand
    {
        get => GetValue(SelectFolderCommandProperty);
        set => SetValue(SelectFolderCommandProperty, value);
    }

}
