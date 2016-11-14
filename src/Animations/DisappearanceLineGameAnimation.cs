using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;


namespace NRTowerDefense
{
    public class DisappearanceLineGameAnimation : DisappearanceUIElementGameAnimation
    {
        static private Line getLine(Point from, Point to, double width, Color color)
        {
            Line line = new Line();

            line.StrokeThickness = width;
            line.Stroke = new SolidColorBrush(color);
            line.StrokeStartLineCap = line.StrokeEndLineCap = PenLineCap.Round;

            line.X1 = from.X;
            line.Y1 = from.Y;
            line.X2 = to.X;
            line.Y2 = to.Y;

            return line;
        }

        public DisappearanceLineGameAnimation(Point from, Point to, double width, Color color,double duration = 5.0, double fromOpacity = 1.0, double toOpacity = 0.0)
            : base
            (
                getLine(from, to, width, color),                
                duration,
                fromOpacity,
                toOpacity
            )
        {
        }
    }
}
