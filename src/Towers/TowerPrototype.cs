using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using Helper;

namespace NRTowerDefense
{
    public class TowerPrototype : Tower
    {
        /// <summary>
        /// Флаг, указывающий на то, может ли башня быть построена на месте прототипа
        /// </summary>
        public bool CanBuild;
        /// <summary>
        /// Целевая башня
        /// </summary>
        public Tower TargetTower;
        /// <summary>
        /// Конструктор
        /// </summary>
        public TowerPrototype(Tower targetTower)
            : base(Mouse.GetPosition(Game.Field.ConnectedCanvas), 0.0, null, 0.0, new GameCell(0.0))
        {
            this.TargetTower = targetTower;
            targetTower.State = GameObjectState.Selected;
        }
        /// <summary>
        /// Реакция на тик таймера
        /// </summary>
        public override void Tick(double interval)
        {
            // обновление положения
            Position = Mouse.GetPosition(Game.Field.ConnectedCanvas);

            #region Прилипание к сетке
            double posX =  (Math.Round((Position.X - Game.Field.Indention) / Game.Field.CellSize)) * Game.Field.CellSize + Game.Field.Indention;
            double posY =  (Math.Round((Position.Y - Game.Field.Indention) / Game.Field.CellSize)) * Game.Field.CellSize + Game.Field.Indention;

            if (Position.X < Game.Field.Indention + Game.Field.CellSize)
                posX = Game.Field.Indention + Game.Field.CellSize;
            else if (Position.X > Game.Field.Indention + Game.Field.CellSize * (Game.Field.GetXCellCount() - 1))
                posX = Game.Field.Indention + Game.Field.CellSize * (Game.Field.GetXCellCount() - 1);

            if (Position.Y < Game.Field.Indention + Game.Field.CellSize)
                posY = Game.Field.Indention + Game.Field.CellSize;
            else if (Position.Y > Game.Field.Indention + Game.Field.CellSize * (Game.Field.GetYCellCount() - 1))
                posY = Game.Field.Indention + Game.Field.CellSize * (Game.Field.GetYCellCount() - 1);

            
            TargetTower.Position = Position = new Point( posX, posY ); 
            #endregion

            #region Обновление флага canBuild
            CanBuild = true;

            if (TargetTower.Cost > Game.Money)
                CanBuild = false;
            else
            {
                #region Проверка расстояний до башен
                foreach (Tower tower in Game.Field.GetTowers())
                {
                    if (!(tower is TowerPrototype))
                        if (HelperFunctions.GetLength(tower.Position, Position) < (TargetTower.Radius + tower.Radius).ToPixels())
                        {
                            CanBuild = false;
                            break;
                        }
                }
                #endregion

                #region Проверка расстояний до монстров
                foreach (Monster monster in Game.Field.GetMonsters())
                {
                    if (HelperFunctions.GetLength(monster.Position, Position) < (TargetTower.Radius + monster.Radius).ToPixels())
                    {
                        CanBuild = false;
                        break;
                    }
                }
                #endregion
            } 
            #endregion
        }

        /// <summary>
        /// Постойка башни на выбранном месте
        /// </summary>
        public void Build()
        {
            Game.ChangeMoney(-1 * TargetTower.Cost);
            TargetTower.State = GameObjectState.Simple;
            Game.Field.AddTower(TargetTower);

            if (TargetTower is IHaveAttackArea)
            {
                (TargetTower as IHaveAttackArea).RemoveAttackArea(Game.Field.ConnectedCanvas);
            }

            foreach (KeyValuePair<string,UIElement> item in visuals)
            {
                Game.Field.ConnectedCanvas.Children.Remove(item.Value);
            }

            Game.Field.RemoveTower(this);
        }

        #region DRAWING BLOCK
        private bool visualsCreated = false;
        private bool attackAreaCreated = false;
        private Dictionary<string, UIElement> visuals = new Dictionary<string, UIElement>();
        /// <summary>
        /// Обновляем уже созданные UIElement's представляющие башню
        /// </summary>
        private void updateVisual(Canvas canvas)
        {
            Rectangle prototype = visuals["prototype"] as Rectangle;

            Canvas.SetLeft(prototype, Position.X - TargetTower.Radius.ToPixels());
            Canvas.SetTop(prototype, Position.Y - TargetTower.Radius.ToPixels());

            Brush canBrush = new SolidColorBrush(Color.FromArgb(100, 0, 255, 0));
            Brush cannotBrush = new SolidColorBrush(Color.FromArgb(100, 255, 0, 0));

            prototype.Fill = CanBuild ? canBrush : cannotBrush;

        }
        /// <summary>
        /// Отрисовка на канвас
        /// </summary>
        /// <param name="canvas"></param>
        public override void Draw(Canvas canvas)
        {
            if (TargetTower is IHaveAttackArea)
            {
                if (attackAreaCreated)
                {
                    (TargetTower as IHaveAttackArea).UpdateAttackArea(Game.Field.ConnectedCanvas);
                } else
                {
                    (TargetTower as IHaveAttackArea).DrawAttackArea(Game.Field.ConnectedCanvas);
                    attackAreaCreated = true;
                }
            }

            if (!visualsCreated)
            {
                Rectangle prototype = new Rectangle();
                prototype.Width = prototype.Height = Game.Field.CellSize * 2;
                prototype.Stroke = new SolidColorBrush(Colors.White);
                prototype.StrokeThickness = 2;
                prototype.RadiusX = prototype.RadiusY = Game.Field.CellSize / 3;
                Panel.SetZIndex(prototype, GameProperties.PrototypeZIndex);
                visuals.Add("prototype", prototype);

                canvas.Children.Add(prototype);

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
            if (TargetTower is IHaveAttackArea)
            {
                (TargetTower as IHaveAttackArea).RemoveAttackArea(canvas);
            }

            foreach (KeyValuePair<string, UIElement> item in visuals)
            {
                canvas.Children.Remove(item.Value);
            }
            visuals.Clear();
        }
        #endregion

    }
}
