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
    public class ColorFieldGameAnimation : DisappearanceUIElementGameAnimation   
    {
        private static UIElement getFillUIElement(Color color)
        {
            Rectangle rectangle = new Rectangle();

            rectangle.Width = Game.Field.ConnectedCanvas.ActualWidth;
            rectangle.Height = Game.Field.ConnectedCanvas.ActualHeight;
            rectangle.RadiusX = rectangle.RadiusY = 6.0;
            rectangle.IsHitTestVisible = false;
            rectangle.Fill = new SolidColorBrush(color);
            
            Panel.SetZIndex(rectangle, GameProperties.ColorFieldAnimationZIndex);

            return rectangle;
        }

        public ColorFieldGameAnimation(Color color, double duration = 2.5, double fromOpacity = 0.25, double toOpacity = 0.0)
            : base ( 
                        getFillUIElement(color),
                        duration,
                        fromOpacity,
                        toOpacity
                    )
        {
        }
    }
}
