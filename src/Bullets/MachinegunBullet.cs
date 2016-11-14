using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Helper;

namespace NRTowerDefense
{
    public class MachinegunBullet : Bullet
    {
        /// <summary>
        /// Жирный конструктор
        /// </summary>
        /// <param name="position"></param>
        /// <param name="target"></param>
        /// <param name="speed"></param>
        /// <param name="damage"></param>
        public MachinegunBullet(Point position, Monster target, GameCell speed, double damage, GameCell damageRadius)
            : base(position, target, speed, damage, damageRadius)
        {
            if (target != null)
            {
                double angleToTargetInRad = (Math.PI / 180.0) * HelperFunctions.GetAngleFromPointToPoint(Position, target.Position);
                Point fromPoint = Position + new Vector(Math.Sin(angleToTargetInRad), -Math.Cos(angleToTargetInRad)) * Game.Field.CellSize;

                Position = fromPoint;
            }
        }
        /// <summary>
        /// Клонирует ракету
        /// </summary>
        /// <returns></returns>
        public override Bullet Clone(Point position, Monster target)
        {
            return new MachinegunBullet(position, target, Speed, Damage, DamageRadius);
        }

        #region DRAWING BLOCK
        private double bulletRadius = Game.Field.CellSize / 1.6;
        private bool visualsCreated = false;
        private Dictionary<string, UIElement> visuals = new Dictionary<string, UIElement>();
        /// <summary>
        /// Обновление UIElement'ов
        /// </summary>
        /// <param name="canvas"></param>
        private void updateVisual(Canvas canvas)
        {
            Image bullet = visuals["bullet"] as Image;

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
                bullet.Source = new BitmapImage(new Uri("/images/bullet7.png", UriKind.Relative));
                bullet.Width = bullet.Height = 2 * bulletRadius;
                bullet.RenderTransformOrigin = new Point(0.5, 0.5);

                visuals.Add("bullet", bullet);

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
