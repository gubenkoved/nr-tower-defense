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
    public class DisappearanceUIElementGameAnimation : GameAnimation
    {
        private DoubleAnimation opacityAnimation;
        private UIElement uielement;


        public DisappearanceUIElementGameAnimation(UIElement uielement, double duration = 5.0, double fromOpacity = 1.0, double toOpacity = 0.0)
        {
            this.uielement = uielement;

            opacityAnimation = new DoubleAnimation();
            opacityAnimation.From = fromOpacity;
            opacityAnimation.To = toOpacity;
            opacityAnimation.Duration = TimeSpan.FromSeconds(duration);

            opacityAnimation.Completed += new EventHandler(Animation_Completed);
        }

        void Animation_Completed(object sender, EventArgs e)
        {
            Finished = true;
        }

        public override void StartAnimation(Canvas canvas)
        {
            visuals.Add(uielement);

            uielement.BeginAnimation(Image.OpacityProperty, opacityAnimation);

            foreach (UIElement item in visuals)
            {
                canvas.Children.Add(item);
            }
        }

        void positionAnimation_Completed(object sender, EventArgs e)
        {
            Finished = true;
        }

        public override void ClearFromCanvas(Canvas canvas)
        {
            foreach (UIElement item in visuals)
            {
                canvas.Children.Remove(item);
            }
            visuals.Clear();
        }
    }
}
