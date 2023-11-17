using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

namespace GamerVII.Launcher.Views.Converters;

public class ListIsEmptyConverter : MarkupExtension, IValueConverter
{
    public override object ProvideValue(IServiceProvider serviceProvider) => this;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is IEnumerable<object> enumerable)
        {
            return !enumerable.Any();
        }

        return true;
    }

}
