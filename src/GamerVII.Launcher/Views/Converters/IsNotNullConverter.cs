using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

namespace GamerVII.Launcher.Views.Converters;

public class IsNotNullConverter : MarkupExtension, IValueConverter
{
    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => null;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is not null;

}
