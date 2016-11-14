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
    public class FreezeTower : Tower, IHaveAttackArea, IUpgradeableTowerWithFreezeBullet
    {
        /// <summary>
        /// Текствовое описание башни
        /// </summary>
        public static new string Description = "Замораживающая башня ($10) ― башня, стреляющая замораживающими снарядами, хорошо подходит для замедления достаточно стойких противников (не действует на монстров с иммунитетом)";

        public int Level { set; get; }
        public int MaxLevel
        {
            get
            {
                return 5;
            }
        }
        #region Upgrade scales
        public double[] CostUpgradeScale
        {
            get
            {
                return new double[] { 0, 40, 150, 250, 500 };
            }
        }
        public double[] ReloadingSpeedUpgradeScale
        {
            get
            {
                return new double[] { 0, 1.3, 1.6, 1.8, 2.0 };
            }
        }
        public double[] BulletDamageUpgradeScale
        {
            get
            {
                return new double[] { 0, 0, 0, 0, 0 };
            }
        }
        public GameCell[] AttackRadiusUpgradeScale
        {
            get
            {
                return new GameCell[] 
                { 
                    new GameCell(0), 
                    new GameCell(5.0),
                    new GameCell(6.0),
                    new GameCell(7.0),
                    new GameCell(8.0)
                };
            }
        }
        public GameCell[] BulletSpeedUpgradeScale
        {
            get
            {
                return new GameCell[] 
                { 
                    new GameCell(0), 
                    new GameCell(5.0),
                    new GameCell(7.0),
                    new GameCell(8.0),
                    new GameCell(9.0)
                };
            }
        }
        public double[] FreezeTimeUpgradeScale
        {
            get
            {
                return new double[] { 0, 2.5, 2.7, 3.0, 3.3 };
            }
        }
        public double[] FreezeFactorUpgradeScale
        {
            get
            {
                return new double[] { 0, 0.45, 0.5, 0.55, 0.6 };
            }
        }
        #endregion
        public void FreezeUpgrade()
        {
            IFreezeBullet b = (Bullet as IFreezeBullet);
            b.FreezeFactor = FreezeFactorUpgradeScale[Level];
            b.FreezeTime = FreezeTimeUpgradeScale[Level];
        }
        public void Upgrade()
        {
            Game.ChangeMoney(-1 * (CostUpgradeScale[Level] - Cost));

            Cost = CostUpgradeScale[Level];
            ReloadingSpeed = ReloadingSpeedUpgradeScale[Level];
            Bullet.Damage = BulletDamageUpgradeScale[Level];
            AttackRadius = AttackRadiusUpgradeScale[Level];
            Bullet.Speed = BulletSpeedUpgradeScale[Level];

            FreezeUpgrade();

            Level += 1;
        }

        public FreezeTower(Point position)
            : base
            (
                position,
                10.0,
                new FreezeBullet(position, null, new GameCell(7.0), new GameCell(0.0), 0.4, 2.0),
                1,
                new GameCell(4.0),
                new NotFreezedMonsterSelectionStrategy()
            )
        {
            Radius = new GameCell(1);
            drawRadius = 1.05 * Radius.ToPixels();
        }

        private bool visualsCreated = false;
        private double drawRadius;
        private Dictionary<string, UIElement> visuals = new Dictionary<string, UIElement>();
        /// <summary>
        /// Обновляем уже созданные UIElement's представляющие башню
        /// </summary>
        private void updateVisual(Canvas canvas)
        {
            UpdateAttackArea(canvas);
        }
        /// <summary>
        /// Рисует область атаки
        /// </summary>
        public void DrawAttackArea(Canvas canvas)
        {
            Ellipse attackArea = new Ellipse();
            Color attackFillColor = Color.FromRgb(120, 120, 120);
            attackFillColor.A = 50;
            attackArea.Fill = new SolidColorBrush(attackFillColor);
            attackArea.Stroke = new SolidColorBrush(Colors.White);
            attackArea.StrokeThickness = 1;
            attackArea.IsHitTestVisible = false;
            Panel.SetZIndex(attackArea, GameProperties.TowerAttackAreaZIndex);
            
            visuals.Add("attackArea", attackArea);
            canvas.Children.Add(attackArea);
        }
        public void UpdateAttackArea(Canvas canvas)
        {
            Ellipse attackArea = visuals["attackArea"] as Ellipse;

            switch (State)
            {
                case GameObjectState.Simple:
                    if (attackArea.Visibility == Visibility.Visible)
                        attackArea.Visibility = Visibility.Collapsed;

                    break;
                case GameObjectState.Selected:
                    if (attackArea.Visibility != Visibility.Visible)
                        attackArea.Visibility = Visibility.Visible;

                    attackArea.Width = attackArea.Height = AttackRadius.ToPixels() * 2;
                    Canvas.SetLeft(attackArea, Position.X - AttackRadius.ToPixels());
                    Canvas.SetTop(attackArea, Position.Y - AttackRadius.ToPixels());

                    break;
            }
        }
        public void RemoveAttackArea(Canvas canvas)
        {
            if (visuals.ContainsKey("attackArea"))
            {
                canvas.Children.Remove(visuals["attackArea"]);
                visuals.Remove("attackArea");
            }
        }
        /// <summary>
        /// Рисует башню
        /// </summary>
        public override void Draw(Canvas canvas)
        {
            if (!visualsCreated)
            {
                #region Draw tower
                Image tower = new Image();
                visuals.Add("tower", tower);
                tower.Source = new BitmapImage(new Uri("/images/tower4.png", UriKind.Relative));
                tower.Width = tower.Height = 2 * drawRadius;
                tower.RenderTransformOrigin = new Point(0.5, 0.5);
                tower.MouseDown += (s, e) => Game.Field.SetAsSelected(this);
                Panel.SetZIndex(tower, GameProperties.TowersZIndex);
                Canvas.SetLeft(tower, Position.X - drawRadius);
                Canvas.SetTop(tower, Position.Y - drawRadius);

                canvas.Children.Add(tower);
                #endregion

                DrawAttackArea(canvas);

                visualsCreated = true;
            }

            updateVisual(canvas);
        }
        /// <summary>
        /// Удаляет все визуальные элементы
        /// </summary>
        public override void ClearFromCanvas(Canvas canvas)
        {
            foreach (KeyValuePair<string, UIElement> item in visuals)
            {
                canvas.Children.Remove(item.Value);
            }
            visuals.Clear();
        }
    }
}
