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
    public class MachinegunTower : Tower, IHaveAttackArea, IUpgradeableTower
    {
        /// <summary>
        /// Текствовое описание башни
        /// </summary>
        public static new string Description = "Пулемётная башня ($20) ― башня, которая, благодаря своей скорострельности, предназначенна для противодействия большим группам монстров";

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
                return new double[] { 0, 100, 200, 400, 1000 };
            }
        }
        public double[] ReloadingSpeedUpgradeScale
        {
            get
            {
                return new double[] { 0, 4.0, 4.5, 5.0, 6.0 };
            }
        }
        public double[] BulletDamageUpgradeScale
        {
            get
            {
                return new double[] { 0, 30, 40, 50, 60 };
            }
        }
        public GameCell[] AttackRadiusUpgradeScale
        {
            get
            {
                return new GameCell[] 
                { 
                    new GameCell(0), 
                    new GameCell(4.5), 
                    new GameCell(4.8), 
                    new GameCell(5.1),
                    new GameCell(5.5)
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
                    new GameCell(13),
                    new GameCell(16),
                    new GameCell(18)
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
        public MachinegunTower(Point position)
            : base
            (
                position, 
                20.0, 
                new MachinegunBullet( position, null, new GameCell(10.0), 10.0, new GameCell( 0.0 ) ), 
                3.0,
                new GameCell( 4.0 )
            )
        {
            Radius = new GameCell( 1 );
            drawRadius = 1.05 * Radius.ToPixels();
            drawGunRadius = 0.9 * drawRadius;
            gunAngle = 0.0;
        }

        /// <summary>
        /// Функция возвращающая истину, если монстр находится в области атаки
        /// </summary>
        public override bool InAttackArea(Monster monster)
        {
            if (HelperFunctions.GetLength(Position, monster.Position) <= AttackRadius.ToPixels())
            {
                if (Math.Abs(monster.Position.X - Position.X) <= Radius.ToPixels() || Math.Abs(monster.Position.Y - Position.Y) <= Radius.ToPixels())
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Реакция на тик таймера
        /// </summary>
        public override void Tick(double interval)
        {
            base.Tick(interval);

            gunAngle += interval * 90.0;
        }
        #region DRAWING BLOCK
        private double gunAngle;
        private double drawRadius;
        private double drawGunRadius;
        private bool visualsCreated = false;
        private Dictionary<string, UIElement> visuals = new Dictionary<string, UIElement>();
        /// <summary>
        /// Обновляем уже созданные UIElement's представляющие башню
        /// </summary>
        /// <param name="canvas"></param>
        private void updateVisual(Canvas canvas)
        {
            Image tower = visuals["tower"] as Image;
            Image gun = visuals["gun"] as Image;

            gun.RenderTransform = new RotateTransform(gunAngle);

            UpdateAttackArea(canvas);
        }
        private void recreateDrawAttackGeometry(Path attackArea)
        {
            RectangleGeometry hRect = new RectangleGeometry();
            hRect.Rect = new Rect(0.0, AttackRadius.ToPixels() - Radius.ToPixels(), 2 * AttackRadius.ToPixels(), 2 * Radius.ToPixels());
            hRect.RadiusX = hRect.RadiusY = 5;

            RectangleGeometry vRect = new RectangleGeometry();
            vRect.Rect = new Rect(AttackRadius.ToPixels() - Radius.ToPixels(), 0.0, 2 * Radius.ToPixels(), 2 * AttackRadius.ToPixels());
            vRect.RadiusX = vRect.RadiusY = 5;

            Geometry crossGeometry = new CombinedGeometry(GeometryCombineMode.Union, hRect, vRect);
            attackArea.Data = crossGeometry;
        }
        /// <summary>
        /// Рисует область атаки
        /// </summary>
        /// <param name="canvas"></param>
        public void DrawAttackArea(Canvas canvas)
        {
            Path attackArea = new Path();

            Color attackFillColor = Color.FromRgb(120, 120, 120);
            attackFillColor.A = 50;
            attackArea.Fill = new SolidColorBrush(attackFillColor);
            attackArea.Stroke = new SolidColorBrush(Colors.White);
            attackArea.StrokeThickness = 1;
            attackArea.IsHitTestVisible = false;

            recreateDrawAttackGeometry(attackArea);
            
            Panel.SetZIndex(attackArea, GameProperties.TowerAttackAreaZIndex);
            visuals.Add("attackArea", attackArea);

            canvas.Children.Add(attackArea);
        }
        public void UpdateAttackArea(Canvas canvas)
        {
            Path attackArea = visuals["attackArea"] as Path;

            

            switch (State)
            {
                case GameObjectState.Simple:
                    if (attackArea.Visibility == Visibility.Visible)
                        attackArea.Visibility = Visibility.Collapsed;
                    break;
                case GameObjectState.Selected:
                    if (attackArea.Visibility != Visibility.Visible)
                        attackArea.Visibility = Visibility.Visible;

                    recreateDrawAttackGeometry(attackArea);
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
                tower.Source = new BitmapImage(new Uri("/images/tower8.png", UriKind.Relative));
                tower.Width = tower.Height = 2 * drawRadius;
                tower.RenderTransformOrigin = new Point(0.5, 0.5);
                tower.MouseDown += (s, e) => Game.Field.SetAsSelected(this);
                Panel.SetZIndex(tower, GameProperties.TowersZIndex);
                Canvas.SetLeft(tower, Position.X - drawRadius);
                Canvas.SetTop(tower, Position.Y - drawRadius);

                Image gun = new Image();
                visuals.Add("gun", gun);
                gun.IsHitTestVisible = false;
                gun.Source = new BitmapImage(new Uri("/images/gun3.png", UriKind.Relative));
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
