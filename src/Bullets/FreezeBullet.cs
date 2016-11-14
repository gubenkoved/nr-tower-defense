using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Helper;

namespace NRTowerDefense
{
    public class FreezeBullet : Bullet, IFreezeBullet
    {
        private double freezeFactor;
        private double freezeTime;

        public double FreezeFactor
        {
            get
            {
                return freezeFactor;
            }
            set
            {
                freezeFactor = value;
            }
        }
        public double FreezeTime
        {
            get
            {
                return freezeTime;
            }
            set
            {
                freezeTime = value;
            }
        }

        /// <summary>
        /// Жирный конструктор
        /// </summary>
        /// <param name="position"></param>
        /// <param name="target"></param>
        /// <param name="speed"></param>
        /// <param name="damage"></param>
        public FreezeBullet(Point position, Monster target, GameCell speed, GameCell damageRadius, double freezeFactor = 0.4, double freezeTime = 2.0 )
            : base(position, target, speed, 0, damageRadius)
        {
            this.freezeFactor = freezeFactor;
            this.freezeTime = freezeTime;
        }
        /// <summary>
        /// Клонирует ракету
        /// </summary>
        /// <returns></returns>
        public override Bullet Clone(Point position, Monster target)
        {
            return new FreezeBullet(position, target, Speed, DamageRadius, freezeFactor, freezeTime);
        }
        /// <summary>
        /// Реакция на тик таймера
        /// </summary>
        /// <param name="interval"></param>
        public override void Tick(double interval)
        {
            base.Tick(interval);

            double needAngle = HelperFunctions.GetAngleFromPointToPoint(Position, Target.Position);
            angle = needAngle;
        }
        #region DRAWING BLOCK
        private double angle;
        private double bulletRadius = Game.Field.CellSize / 2.0;
        private bool visualsCreated = false;
        private Dictionary<string, UIElement> visuals = new Dictionary<string, UIElement>();
        /// <summary>
        /// Обновление UIElement'ов
        /// </summary>
        /// <param name="canvas"></param>
        private void updateVisual(Canvas canvas)
        {
            Image bullet = visuals["bullet"] as Image;

            bullet.RenderTransform = new RotateTransform(angle);

            Canvas.SetLeft(bullet, Position.X - bulletRadius);
            Canvas.SetTop(bullet, Position.Y - bulletRadius);
        }
        /// <summary>
        /// Отрисовывает себя на канвас
        /// </summary>
        /// <param name="canvas"></param>
        public override void Draw(Canvas canvas)
        {
            if (!visualsCreated)
            {
                Image bullet = new Image();
                bullet.IsHitTestVisible = false;
                bullet.Source = new BitmapImage(new Uri("/images/bullet6.png", UriKind.Relative));
                bullet.Width = bullet.Height = 2 * bulletRadius;
                bullet.RenderTransformOrigin = new Point(0.5, 0.5);

                visuals.Add("bullet",bullet);

                canvas.Children.Add(bullet);

                visualsCreated = true;
            }

            updateVisual(canvas);
        }
        /// <summary>
        /// Удаляет все визуальные элементы
        /// </summary>
        /// <param name="canvas"></param>
        public override void ClearFromCanvas(Canvas canvas)
        {
            foreach (KeyValuePair<string, UIElement> item in visuals)
            {
                canvas.Children.Remove(item.Value);
            }
            visuals.Clear();
        }
        #endregion
    }
}
