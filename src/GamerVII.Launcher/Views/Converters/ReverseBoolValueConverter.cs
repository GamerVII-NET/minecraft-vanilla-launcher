using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

namespace GamerVII.Launcher.Views.Converters;

public class ReverseBoolValueConverter : MarkupExtension, IValueConverter
{
    public override object ProvideValue(IServiceProvider serviceProvider) => this;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => value;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool booleanValue)
            return !booleanValue;

        return value;
    }

}
