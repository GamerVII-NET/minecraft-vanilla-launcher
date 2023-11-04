using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace GamerVII.Launcher.Views.Components;

public class DownloadStatusComponent : TemplatedControl
{

    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<DownloadStatusComponent, string>(
        nameof(Title), "Заголовок");

    public static readonly StyledProperty<string> DescriptionProperty = AvaloniaProperty.Register<DownloadStatusComponent, string>(
        nameof(Description), "Описание");

    public static readonly StyledProperty<string> PercentageProperty = AvaloniaProperty.Register<DownloadStatusComponent, string>(
        nameof(Percentage), "100");

    public string Percentage
    {
        get => GetValue(PercentageProperty);
        set => SetValue(PercentageProperty, value);
    }

    public string Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

}

