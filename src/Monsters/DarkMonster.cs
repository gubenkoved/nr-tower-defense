using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Helper;

namespace NRTowerDefense
{
    public class DarkMonster : Monster
    {
        /// <summary>
        /// Текствовое описание монстра
        /// </summary>
        public static new string Description = "Живучий монстр";
        /// <summary>
        /// Конструктор
        /// </summary>
        public DarkMonster(GameCell speed, double life = 1000.0, double cost = 50.0, double armour = 10.0, double rotSpeed = 270.0)
            :  this(Game.Field.StartMonsterPosition, Game.Field.TargetMonsterPosition, speed, life, cost, armour, rotSpeed)
        {
        }
        /// <summary>
        /// "Жирный" конструктор
        /// </summary>
        public DarkMonster(Point position, Point target, GameCell speed, double life, double cost, double armour, double rotationSpeed)
            : base(position, 0, speed, life, cost, armour, target, rotationSpeed)
        {
            Radius = new GameCell(0.5);
            drawMonseterRaduis = Radius.ToPixels() * 1.4;
        }
        /// <summary>
        /// Возвращает копию монстра
        /// </summary>
        /// <returns></returns>
        public override Monster Clone()
        {
            return new DarkMonster(Position, Target, Speed, StartLife, Cost, Armour, RotationSpeed);
        }

        #region DRAWING BLOCK
        private double drawMonseterRaduis;
        private bool visualsCreated = false;
        private Dictionary<string, UIElement> visuals = new Dictionary<string, UIElement>();
        private void updateVisual(Canvas canvas)
        {
            Image monster = visuals["monster"] as Image;

            Canvas.SetLeft(monster, Position.X - monster.Width / 2.0);
            Canvas.SetTop(monster, Position.Y - monster.Height / 2.0);
            monster.RenderTransform = new RotateTransform(Angle);
        }
        /// <summary>
        /// Отрисовка себя на канвас
        /// </summary>
        /// <param name="canvas"></param>
        public override void Draw(Canvas canvas)
        {
            if (!visualsCreated)
            {
                int zIndex = Game.Field.GetLastMonsterZIndex();

                Image monster = new Image();
                monster.Source = new BitmapImage(new Uri("/images/monster4.png", UriKind.Relative));
                monster.Width = monster.Height = 2.0 * drawMonseterRaduis;
                monster.RenderTransformOrigin = new Point(0.5, 0.5);
                monster.MouseDown += (s, e) => Game.Field.SetAsSelected(this);
                Panel.SetZIndex(monster, zIndex);
                visuals.Add("monster", monster);

                foreach (KeyValuePair<string, UIElement> item in visuals)
                {
                    canvas.Children.Add(item.Value);
                }

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
