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
    /// Абстрактный класс башня
    /// </summary>
    public abstract class Tower : GameObject
    {
        /// <summary>
        /// Текствовое описание башни
        /// </summary>
        public static string Description = "Описание башни";
        /// <summary>
        /// Цена башни
        /// </summary>
        public double Cost { get; protected set; }
        /// <summary>
        /// Скорость перезарядки
        /// </summary>
        public double ReloadingSpeed { get; protected set; }
        /// <summary>
        /// Время до окончания перезарядки
        /// </summary>
        public double TimeToReloading { get; protected set; }
        /// <summary>
        /// Радиус поражения
        /// </summary>
        public GameCell AttackRadius { get; protected set; }
        /// <summary>
        /// Целевой монстр башни
        /// </summary>
        protected Monster target;
        /// <summary>
        /// Снаряды, которыми стреляет башня
        /// </summary>
        public Bullet Bullet { get; protected set; }
        /// <summary>
        /// Стратегия выбора цели из наступающих монстров
        /// </summary>
        protected MonsterSelectionStrategy MonsterSelectionStrategy;
        /// <summary>
        /// Конструктор башни
        /// </summary>
        /// <param name="position">Положение башни</param>
        public Tower(Point position, double cost, Bullet bullet, double reloadingSpeed, GameCell attackRadius, MonsterSelectionStrategy monsterSelectionStrategy = null)
        {
            this.Position = position;
            this.Cost = cost;
            this.Bullet = bullet;
            this.ReloadingSpeed = reloadingSpeed;
            this.AttackRadius = attackRadius;

            if (monsterSelectionStrategy == null)
                MonsterSelectionStrategy = new FirstMonsterSelectionStrategy();
            else
                MonsterSelectionStrategy = monsterSelectionStrategy;

            State = GameObjectState.Simple;

            if (this is IUpgradeableTower)
            {
                (this as IUpgradeableTower).Level = 1;
            }
        }
        /// <summary>
        /// Переопределяемая реакция на тик
        /// </summary>
        public override void Tick(double interval)
        {
            // обновляем время до выстрела
            TimeToReloading = Math.Max(0.0, TimeToReloading - interval);

            // если цель удалилась из области атаки, или была убита, или будет убита летящими ракетами, то снимаем её
            if (target != null)
            {
                if (
                        target.Killed
                        || Game.Field.CalculateSummaryDamage(target, Game.Field.GetBulletsWithTarget(target)) >= target.Life
                        || !InAttackArea(target)
                    )
                    target = null;
            }

            // если цель не задана или того требует стратегия поиска целей, то пытаемся найти новую
            if (target == null || MonsterSelectionStrategy.EachTickRecalculateTarget)
            {
                List<Monster> inAttackArea = new List<Monster>();
                foreach (Monster monster in Game.Field.GetMonsters())
                {
                    if (InAttackArea(monster))
                        inAttackArea.Add(monster);
                }
                target = MonsterSelectionStrategy.Select(inAttackArea);
            }

            // стреляем если есть цель, и башня готова к выстрелу
            if (target != null && TimeToReloading == 0.0)
            {
                Game.Field.AddBullet(Bullet.Clone(Position, target));
                TimeToReloading = 1.0 / ReloadingSpeed;
            }
        }
        /// <summary>
        /// Функция возвращающая истину, если монстр находится в области атаки
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public virtual bool InAttackArea( Monster monster )
        {
            if (HelperFunctions.GetLength(Position, monster.Position) <= AttackRadius.ToPixels())
            {
                return true;
            }

            return false;
        }
    }
}
