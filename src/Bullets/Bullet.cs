using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Helper;
using System.Windows.Controls;

namespace NRTowerDefense
{
    /// <summary>
    /// Абстрактный класс ракета
    /// </summary>
    public abstract class Bullet : GameObject
    {
        /// <summary>
        /// Целевой монстр
        /// </summary>
        public Monster Target;
        /// <summary>
        /// Скорость ракеты
        /// </summary>
        public GameCell Speed;
        /// <summary>
        /// Повреждение
        /// </summary>
        public double Damage;
        /// <summary>
        /// Радиус поражения
        /// </summary>
        public GameCell DamageRadius;
        /// <summary>
        /// Попала ли ракета в цель
        /// </summary>
        public bool Hit = false;
        /// <summary>
        /// Действия по истечению тика
        /// </summary>
        public override void Tick(double interval)
        {
            if (Hit)
                return;

            double needAngle = HelperFunctions.GetAngleFromPointToPoint(Position, Target.Position);
            double needGoing = HelperFunctions.GetLength(Position, Target.Position);
            double mayGoing = Speed.ToPixels() * interval;

            if (mayGoing < needGoing)
            {
                Position = Position
                    + new Vector(
                            +mayGoing * Math.Sin(Math.PI * needAngle / 180.0),
                            -mayGoing * Math.Cos(Math.PI * needAngle / 180.0)
                        );
                return;
            }
            else
            {
                Position = Target.Position;

                Hit = true;

                #region Наносим повреждения всем монстрам в зоне поражения
                foreach (Monster m in Game.Field.GetMonsters())
                {
                    if (HelperFunctions.GetLength(Position, m.Position) <= DamageRadius.ToPixels())
                    {
                        if (!m.Killed)
                        {
                            m.Hit(this);
                        }
                    }
                } 
                #endregion
            }
        }
        /// <summary>
        /// "Жирный" конструктор
        /// </summary>
        public Bullet(Point position, Monster target, GameCell speed, double damage, GameCell damageRadius)
        {
            this.Position = position;
            this.Target = target;
            this.Speed = speed;
            this.Damage = damage;
            this.DamageRadius = damageRadius;
            this.Radius = new GameCell(double.NaN);
        }
        /// <summary>
        /// Клонирует ракету
        /// </summary>
        /// <returns></returns>
        public abstract Bullet Clone(Point position, Monster target);
    }
}
