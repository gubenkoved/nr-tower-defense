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
    public class SimpleTower : Tower, IHaveAttackArea, IUpgradeableTower
    {
        /// <summary>
        /// Текствовое описание башни
        /// </summary>
        public static new string Description = "Ракетная башня ($10) ―  самый универсальный тип башни, который может быть использован";

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
                return new double[] { 0, 30, 100, 300, 600 };
            }
        }
        public double[] ReloadingSpeedUpgradeScale
        {
            get
            {
                return new double[] { 0, 1.5, 2.0, 2.2, 2.5 };
            }
        }
        public double[] BulletDamageUpgradeScale
        {
            get
            {
                return new double[] { 0, 20, 30, 50, 70 };
            }
        }
        public GameCell[] AttackRadiusUpgradeScale
        {
            get
            {
                return new GameCell[] 
                { 
                    new GameCell(0), 
                    new GameCell(4.0), 
                    new GameCell(5.0), 
                    new GameCell(6.0),
                    new GameCell(7.0)
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
                    new GameCell(10), 
                    new GameCell(10),
                    new GameCell(15),
                    new GameCell(15)
                };
            }
        } 
        #endregion
        public void Upgrade()
        {
            Game.ChangeMoney(-1 * (CostUpgradeScale[Level] - Cost));

            Cost = CostUpgradeScale[Level];
            ReloadingSpeed = ReloadingSpeedUpgradeScale[Level];
            Bullet.Damage = BulletDamageUpgradeScale[Level];
            AttackRadius = AttackRadiusUpgradeScale[Level];
            Bullet.Speed = BulletSpeedUpgradeScale[Level];

            Level += 1;
        }

        /// <summary>
        /// Конструктор башни
        /// </summary>
        /// <param name="position">Положение башни</param>
        public SimpleTower (Point position)
            : base
            (
                position, 
                10.0,
                new SimpleBullet( position, null, new GameCell(10.0), 10.0, new GameCell( 0.0 ) ), 
                1.0, 
                new GameCell( 3.5 )
            )
        {
            Radius = new GameCell( 1 );
            drawRadius = 1.05 * Radius.ToPixels();
            drawGunRadius = 1.0 * drawRadius;

            Random rnd = new Random();
            angle = rnd.NextDouble() * 360.0;
        }
        /// <summary>
        /// Реакция на тик
        /// </summary>
        public override void Tick(double interval)
        {
            base.Tick(interval);

            if (target != null)
            {
                angle = HelperFunctions.GetAngleFromPointToPoint(Position, target.Position);
                angleChanged = true;
            }
        }
        #region DRAWING BLOCK
        private double angle = 0;
        private bool angleChanged = true;
        private double drawGunRadius;
        private double drawRadius;
        private bool visualsCreated = false;
        private Dictionary<string, UIElement> visuals = new Dictionary<string, UIElement>();
        /// <summary>
        /// Обновляем уже созданные UIElement's представляющие башню
        /// </summary>
        private void updateVisual(Canvas canvas)
        {
            Image tower = visuals["tower"] as Image;
            Image gun = visuals["gun"] as Image;

            if (angleChanged)
            {
                gun.RenderTransform = new RotateTransform(angle);
            }

            UpdateAttackArea(canvas);
        }
        /// <summary>
        /// Рисует область атаки
        /// </summary>
        public void DrawAttackArea(Canvas canvas)
        {
            Ellipse attackArea = new Ellipse();

            visuals.Add("attackArea", attackArea);
            Color attackFillColor = Color.FromRgb(120, 120, 120);
            attackFillColor.A = 50;
            attackArea.Fill = new SolidColorBrush(attackFillColor);
            attackArea.Stroke = new SolidColorBrush(Colors.White);
            attackArea.StrokeThickness = 1;
            attackArea.IsHitTestVisible = false;
            attackArea.Visibility = Visibility.Collapsed;
            
            Panel.SetZIndex(attackArea, GameProperties.TowerAttackAreaZIndex);
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
        /// Рисует сам себя на канвас
        /// </summary>
        public override void Draw(Canvas canvas)
        {
            if (!visualsCreated)
            {
                #region Draw tower
                Image tower = new Image();
                visuals.Add("tower", tower);
                
                tower.Source = new BitmapImage(new Uri("/images/tower3.png", UriKind.Relative));
                tower.Width = tower.Height = 2 * drawRadius;
                tower.RenderTransformOrigin = new Point(0.5, 0.5);
                tower.MouseDown += (s, e) => Game.Field.SetAsSelected(this);
                
                Panel.SetZIndex(tower, GameProperties.TowersZIndex);
                Canvas.SetLeft(tower, Position.X - drawRadius);
                Canvas.SetTop(tower, Position.Y - drawRadius);
                
                Image gun = new Image();
                visuals.Add("gun", gun);
                
                gun.IsHitTestVisible = false;
                gun.Source = new BitmapImage(new Uri("/images/gun4.png", UriKind.Relative));
                gun.Width = gun.Height = 2 * drawGunRadius;
                gun.RenderTransformOrigin = new Point(0.5, 0.5);
                
                Panel.SetZIndex(gun, GameProperties.TowersGunZIndex);
                Canvas.SetLeft(gun, Position.X - drawGunRadius);
                Canvas.SetTop(gun, Position.Y - drawGunRadius);

                canvas.Children.Add(tower);
                canvas.Children.Add(gun);
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
        #endregion
    }
}
