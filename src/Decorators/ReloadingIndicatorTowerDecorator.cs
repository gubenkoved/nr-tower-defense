using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;

namespace NRTowerDefense
{
    public class ReloadingIndicatorTowerDecorator : TowerDecorator
    {
        private Rectangle reloadingIndicator;

        private void createReloadingIndicator()
        {
            reloadingIndicator = new Rectangle();
            reloadingIndicator.Height = GameProperties.TowersReloadingIndicatorHeight;
            reloadingIndicator.Stroke = new SolidColorBrush(Colors.Black);
            reloadingIndicator.StrokeThickness = 0.5;
            reloadingIndicator.Fill = new SolidColorBrush(Colors.Red);
            reloadingIndicator.RadiusX = reloadingIndicator.RadiusY = 1.0;
            Panel.SetZIndex(reloadingIndicator, GameProperties.TowersReloadingIndicatorZIndex);
        }

        public ReloadingIndicatorTowerDecorator(Tower component, bool enable = true)
            : base(component)
        {
            Enabled = enable;

            createReloadingIndicator();

            Game.Field.ConnectedCanvas.Children.Add(reloadingIndicator);
        }

        public ReloadingIndicatorTowerDecorator(TowerDecorator component, bool enable = true)
            : base(component)
        {
            Enabled = enable;

            createReloadingIndicator();

            Game.Field.ConnectedCanvas.Children.Add(reloadingIndicator);
        }

        public override void Tick(double interval)
        {
            base.Tick(interval);

            reloadingIndicator.Width = (Kernel.TimeToReloading * Kernel.ReloadingSpeed) * 2 * Radius.ToPixels();

            Canvas.SetLeft(reloadingIndicator, Position.X - Radius.ToPixels());
            Canvas.SetTop(reloadingIndicator, Position.Y + Radius.ToPixels() - reloadingIndicator.Height);
        }
        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);

            if (Enabled)
            {
                if (reloadingIndicator.Visibility != Visibility.Visible)
                    reloadingIndicator.Visibility = Visibility.Visible;
            }
            else
            {
                if (reloadingIndicator.Visibility == Visibility.Visible)
                    reloadingIndicator.Visibility = Visibility.Hidden;
            }
        }
        public override void ClearFromCanvas(Canvas canvas)
        {
            base.ClearFromCanvas(canvas);

            Game.Field.ConnectedCanvas.Children.Remove(reloadingIndicator);
        }
    }
}
