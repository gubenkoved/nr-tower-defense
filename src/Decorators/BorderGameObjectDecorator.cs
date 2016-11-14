using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;

namespace NRTowerDefense
{
    public class BorderGameObjectDecorator: GameObjectDecorator
    {
        private double startThickness = 2.0;
        private DoubleCollection startStrokeDashArray = new DoubleCollection() { 3.0, 2.0 };

        private Ellipse border;

        private void createBorder()
        {
            border = new Ellipse();

            border.Stroke = Brushes.White;
            border.StrokeThickness = startThickness;
            border.StrokeDashArray = startStrokeDashArray.Clone();
            border.Width = border.Height = 2.5 * Kernel.Radius.ToPixels();
            border.RenderTransformOrigin = new Point(0.5, 0.5);
            Panel.SetZIndex(border, GameProperties.BorderDecoratorZIndex);

            border.Visibility = Visibility.Hidden;
        }

        public BorderGameObjectDecorator(GameObject gameObject)
            : base( gameObject )
        {
            createBorder();

            Game.Field.ConnectedCanvas.Children.Add(border);
        }

        public override void Tick(double interval)
        {
            base.Tick(interval);

            if (Enabled)
            {
                border.RenderTransform = new RotateTransform(Game.Timer.SecondsElapsed() * 150.0);
                border.StrokeThickness = (0.5 * Math.Sin(Game.Timer.SecondsElapsed() * 10) + 1) + 0.5;
                border.StrokeDashArray[0] = startStrokeDashArray[0] * startThickness / border.StrokeThickness;
                border.StrokeDashArray[1] = startStrokeDashArray[1] * startThickness / border.StrokeThickness;

                Canvas.SetLeft(border, Position.X - (border as Ellipse).Width / 2.0);
                Canvas.SetTop(border, Position.Y - (border as Ellipse).Height / 2.0);

                if (border.Visibility != Visibility.Visible)
                    border.Visibility = Visibility.Visible;
            }
            else
            {
                if (border.Visibility == Visibility.Visible)
                    border.Visibility = Visibility.Hidden;
            }
        }

        public override void ClearFromCanvas(Canvas canvas)
        {
            base.ClearFromCanvas(canvas);

            Game.Field.ConnectedCanvas.Children.Remove(border);
        }
    }
}
