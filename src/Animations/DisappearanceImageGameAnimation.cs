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
    public class DisappearanceImageGameAnimation : DisappearanceUIElementGameAnimation
    {
        static private Image getImageFromUri(Uri uri, Point position, double imageRaduis)
        {
            Image image = new Image();

            image.Source = new BitmapImage(uri);                       

            if (double.IsNaN(imageRaduis))
            {
                image.Width = image.Height = Game.Field.CellSize;
            }
            else
            {
                image.Width = image.Height = 2 * imageRaduis;
            }

            Panel.SetZIndex(image, GameProperties.DisappearanceImageGameAnimationZIndex);
            Canvas.SetLeft(image, position.X - image.Width / 2.0);
            Canvas.SetTop(image, position.Y - image.Height / 2.0);

            return image;
        }

        public DisappearanceImageGameAnimation(Uri imageUri, Point position, double duration = 5.0, double imageRadius = double.NaN, double fromOpacity = 1.0, double toOpacity = 0.0)
            : base
            (
                getImageFromUri(imageUri, position,imageRadius), 
                duration, 
                fromOpacity, 
                toOpacity
            )
        {            
        }
    }
}
