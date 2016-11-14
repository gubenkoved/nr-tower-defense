using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace NRTowerDefense
{
    public class TextGameAnimation : GameAnimation
    {
        private DoubleAnimation positionAnimation;
        private DoubleAnimation opacityAnimation;
        /// <summary>
        /// Отображаемый текст
        /// </summary>
        private string text;
        /// <summary>
        /// Цвет текста
        /// </summary>
        private Color color;
        /// <summary>
        /// Размер шрифта
        /// </summary>
        private double textSize;
        /// <summary>
        /// Позиция анимации
        /// </summary>
        private Point position;

        public TextGameAnimation(string text, Point position, Color color, double duration = 2.0, double textSize = 18.0)
        {
            this.text = text;
            this.position = position;
            this.color = color;
            this.textSize = textSize;

            positionAnimation = new DoubleAnimation();
            positionAnimation.By = 1.5 * Game.Field.CellSize;
            positionAnimation.Duration = TimeSpan.FromSeconds(duration);

            positionAnimation.Completed += new EventHandler(Animation_Completed);

            opacityAnimation = new DoubleAnimation();
            opacityAnimation.To = 0.0;
            opacityAnimation.Duration = TimeSpan.FromSeconds(duration);
        }

        public override void StartAnimation(Canvas canvas)
        {
            TextBlock textBlock = new TextBlock();
            visuals.Add(textBlock);
            textBlock.IsHitTestVisible = false;
            textBlock.Text = text;
            textBlock.FontSize = textSize;
            textBlock.Foreground = new SolidColorBrush(color);
            textBlock.FontFamily = new FontFamily("Century Gothic");            
            Canvas.SetTop(textBlock, position.Y);
            Canvas.SetLeft(textBlock, position.X);
            Panel.SetZIndex(textBlock, int.MaxValue);

            textBlock.BeginAnimation(Canvas.LeftProperty, positionAnimation);
            textBlock.BeginAnimation(Canvas.TopProperty, positionAnimation);
            textBlock.BeginAnimation(TextBlock.OpacityProperty, opacityAnimation);
            
            foreach (UIElement item in visuals)
            {
                canvas.Children.Add(item);
            }
        }

        void Animation_Completed(object sender, EventArgs e)
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
