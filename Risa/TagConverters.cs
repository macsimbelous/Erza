using Avalonia.Data.Converters;
using Avalonia.Media;
using ErzaLib2;
using System;

namespace Risa.Converters;

public static class TagConverters
{
    public static readonly IValueConverter TypeToBrushConverter =
        new FuncValueConverter<TagType, IBrush>(type => type switch
        {
            TagType.General => Brushes.DodgerBlue,
            TagType.Artist => Brushes.Crimson,
            TagType.Studio => Brushes.MediumPurple,
            TagType.Copyright => Brushes.DarkOrchid,
            TagType.Character => Brushes.ForestGreen,
            TagType.Circle => Brushes.OrangeRed,
            TagType.Faults => Brushes.Red,
            TagType.Medium => Brushes.Teal,
            TagType.Meta => Brushes.DarkOrange,
            _ => Brushes.Gray
        });
}
