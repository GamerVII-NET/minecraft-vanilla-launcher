using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using GamerVII.Launcher.Models.Client;

namespace GamerVII.Launcher.Views.Converters;

public class ReleaseDateFromMinecraftVersionConverter : MarkupExtension, IValueConverter
{
    public override object ProvideValue(IServiceProvider serviceProvider) => this;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not MinecraftVersion minecraftVersion) return string.Empty;

        return minecraftVersion.MVersion != null
            ? minecraftVersion.MVersion.ReleaseTime?.ToString("dd.MM.yyyy в HH:mm")
            : string.Empty;
    }

}
