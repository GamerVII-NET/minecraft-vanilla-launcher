﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace GamerVII.Launcher.Views.Components;

public class BadgeComponent : TemplatedControl
{
    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<BadgeComponent, string>(
        nameof(Text), "Badge text");

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

}

