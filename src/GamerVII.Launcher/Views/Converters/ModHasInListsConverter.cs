using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using GamerVII.Launcher.Models.Client;

namespace GamerVII.Launcher.Views.Converters;

public class ModHasInListsConverter : MarkupExtension, IMultiValueConverter
{
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.FirstOrDefault() is not IMod mod) return false;
        
        var modsLists = new List<IMod>();

        if (values.Count > 1 && values[1] is ICollection<IMod> mods)
            modsLists.AddRange(mods);

        if (values.Count > 2 && values[2] is ICollection<IMod> mods2)
            modsLists.AddRange(mods2);

        return modsLists.Contains(mod);


    }
}
