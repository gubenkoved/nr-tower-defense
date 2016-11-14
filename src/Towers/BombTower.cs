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
    public class BombTower : Tower, IHaveAttackArea, IUpgradeableTower, IExplodeTower
    {
        /// <summary>
        /// Текствовое описание башни
        /// </summary>
        public static new string Description = "Башня-бомба ($50) ― взрывающаяся башня, которая хорошо подходит для обороны от больших групп монстров";

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
                return new double[] { 0, 150, 300, 500, 1000 };
            }
        }
        public double[] ReloadingSpeedUpgradeScale
        {
            get
            {
                return new double[] { double.NaN, double.NaN, double.NaN, double.NaN, double.NaN };
            }
        }
        public double[] BulletDamageUpgradeScale
        {
            get
            {
                return new double[] { 0, 400, 800, 1600, 3200 };
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
                    new GameCell(4.5), 
                    new GameCell(5.0),
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
                    new GameCell(16), 
                    new GameCell(17),
                    new GameCell(19),
                    new GameCell(20)
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
        public BombTower(Point position)
            : base
            (
                position, 
                50.0, 
                new SplinterBullet(position, null, new GameCell(15), 100.0, new GameCell( 0.0 ) ), 
                double.NaN, 
                new GameCell( 3.5 )
            )
        {

            Radius = new GameCell(1.0);
            drawRadius = 1.05 * Radius.ToPixels();
        }
        /// <summary>
        /// Реакция на тик
        /// </summary>
        public override void Tick(double interval)
        {
            return;
        }
        /// <summary>
        /// Флаг - была ли взорвана башня
        /// </summary>       
        public bool Exploded { get; private set; }
        /// <summary>
        /// Взорвать башню
        /// </summary>
        public void Explode()
        {
            Exploded = true;

            List<Monster> toAttack = new List<Monster>();
            foreach (Monster monster in Game.Field.GetMonsters())
            {
                if (HelperFunctions.GetLength(Position, monster.Position) <= AttackRadius.ToPixels())
                {
                    toAttack.Add(monster);
                }
            }

            foreach (Monster monster in toAttack)
            {
                Game.Field.AddBullet(Bullet.Clone(Position, monster));
            }

            Game.Field.RecalculateAllMonsterPaths(true);

            Game.Field.AddAnimation(new DisappearanceImageGameAnimation(new Uri("/images/bomb.png", UriKind.Relative), Position, 10.0, Game.Field.CellSize));
            Game.Field.AddAnimation(new ColorFieldGameAnimation(Colors.Yellow, 1.0, 0.5, 0.0));
        }
        #region DRAWING BLOCK
        private double drawRadius;
        private bool visualsCreated = false;
        private Dictionary<string, UIElement> visuals = new Dictionary<string, UIElement>();
        /// <summary>
        /// Обновляем уже созданные UIElement's представляющие башню
        /// </summary>
        /// <param name="canvas"></param>
        private void updateVisual(Canvas canvas)
        {
            Image tower = visuals["tower"] as Image;

            UpdateAttackArea(canvas);
        }
        /// <summary>
        /// Рисует область атаки
        /// </summary>
        /// <param name="canvas"></param>
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
        /// Рисует сам себя на канвас
        /// </summary>
        public override void Draw(Canvas canvas)
        {
            if (!visualsCreated)
            {
                #region Draw tower
                Image tower = new Image();
                visuals.Add("tower", tower);
                tower.Source = new BitmapImage(new Uri("/images/tower5.png", UriKind.Relative));
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
