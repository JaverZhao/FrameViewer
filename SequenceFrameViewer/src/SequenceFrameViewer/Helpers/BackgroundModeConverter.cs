using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SequenceFrameViewer.Helpers;

public class BackgroundModeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string mode)
            return new SolidColorBrush(Color.FromRgb(0x1A, 0x1A, 0x1A));

        return mode switch
        {
            "Black" => new SolidColorBrush(Colors.Black),
            "White" => new SolidColorBrush(Colors.White),
            "Gray" => new SolidColorBrush(Color.FromRgb(0x80, 0x80, 0x80)),
            _ => CreateCheckerboardBrush()
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private static DrawingBrush CreateCheckerboardBrush()
    {
        var checkerSize = 16.0;
        var drawingGroup = new DrawingGroup();

        drawingGroup.Children.Add(
            new GeometryDrawing
            {
                Geometry = new RectangleGeometry(new(0, 0, checkerSize * 2, checkerSize * 2)),
                Brush = Brushes.White
            });

        drawingGroup.Children.Add(
            new GeometryDrawing
            {
                Geometry = new RectangleGeometry(new(0, 0, checkerSize, checkerSize)),
                Brush = new SolidColorBrush(Color.FromRgb(0xCC, 0xCC, 0xCC))
            });

        drawingGroup.Children.Add(
            new GeometryDrawing
            {
                Geometry = new RectangleGeometry(new(checkerSize, checkerSize, checkerSize, checkerSize)),
                Brush = new SolidColorBrush(Color.FromRgb(0xCC, 0xCC, 0xCC))
            });

        var brush = new DrawingBrush
        {
            Drawing = drawingGroup,
            TileMode = TileMode.Tile,
            Viewport = new(0, 0, checkerSize * 2, checkerSize * 2),
            ViewportUnits = BrushMappingMode.Absolute,
            Stretch = Stretch.None
        };

        brush.Freeze();
        return brush;
    }
}
