using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace NRTowerDefense
{
    public class LifeIndicatorMonsterDecorator : MonsterDecorator
    {
        private Rectangle lifeIndicator;

        private void createLifeIndicator()
        {
            lifeIndicator = new Rectangle();
            lifeIndicator.Height = 0.25 * Radius.ToPixels();
            lifeIndicator.Fill = new SolidColorBrush(Colors.Green);
            lifeIndicator.RadiusX = lifeIndicator.RadiusY = 1.0;
            Panel.SetZIndex(lifeIndicator, Game.Field.GetLastMonsterZIndex());
        }

        public LifeIndicatorMonsterDecorator(Monster component, bool enable = true)
            : base( component )
        {
            Enabled = enable;

            createLifeIndicator();

            Game.Field.ConnectedCanvas.Children.Add(lifeIndicator);
        }

        public override void Tick(double interval)
        {
            base.Tick(interval);

            lifeIndicator.Width = (Kernel.Life / Kernel.StartLife) * 2 * Radius.ToPixels();

            Canvas.SetLeft(lifeIndicator, Position.X - Radius.ToPixels());
            Canvas.SetTop(lifeIndicator, Position.Y + Radius.ToPixels());
        }
        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);

            if (Enabled)
            {
                if (lifeIndicator.Visibility != Visibility.Visible)
                    lifeIndicator.Visibility = Visibility.Visible;
            }
            else
            {
                if (lifeIndicator.Visibility == Visibility.Visible)
                    lifeIndicator.Visibility = Visibility.Hidden;
            }
        }
        public override void ClearFromCanvas(Canvas canvas)
        {
            base.ClearFromCanvas(canvas);

            Game.Field.ConnectedCanvas.Children.Remove(lifeIndicator);
        }
    }
}
