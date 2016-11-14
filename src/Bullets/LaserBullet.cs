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
    public class LaserBullet : Bullet
    {
         /// <summary>
        /// Жирный конструктор
        /// </summary>
        /// <param name="position"></param>
        /// <param name="target"></param>
        /// <param name="speed"></param>
        /// <param name="damage"></param>
        public LaserBullet(Point position, Monster target, double damage, GameCell damageRadius)
            : base(position, target, new GameCell(double.PositiveInfinity), damage, damageRadius)
        {
            if (target != null)
            {
                double angleToTargetInRad = (Math.PI / 180.0) *  HelperFunctions.GetAngleFromPointToPoint(Position, target.Position);

                Point fromPoint = Position + new Vector(Math.Sin(angleToTargetInRad), -Math.Cos(angleToTargetInRad)) * Game.Field.CellSize;

                DisappearanceLineGameAnimation hitAnimation =
                        new DisappearanceLineGameAnimation(fromPoint, Target.Position, 4.5, Colors.Red, 0.3, 0.9, 0.0);

                Game.Field.AddAnimation(hitAnimation);
            }
        }
        /// <summary>
        /// Клонирует ракету
        /// </summary>
        /// <returns></returns>
        public override Bullet Clone(Point position, Monster target)
        {
            return new LaserBullet(position, target, Damage, DamageRadius);
        }
        #region DRAWING BLOCK
        /// <summary>
        /// Отрисовывает себя на канвас
        /// </summary>
        /// <param name="canvas"></param>
        public override void Draw(Canvas canvas)
        {
            return;
        }
        /// <summary>
        /// Удаляет все визуальные элементы
        /// </summary>
        /// <param name="canvas"></param>
        public override void ClearFromCanvas(Canvas canvas)
        {
            return;
        }
        #endregion
    }
}
