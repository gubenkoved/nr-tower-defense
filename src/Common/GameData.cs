using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace NRTowerDefense
{
    public class GameData
    {
        /// <summary>
        /// Количество ячеек по оси X
        /// </summary>
        public int XCellCount;
        /// <summary>
        /// Количество ячеек по оси Y
        /// </summary>
        public int YCellCount;
        /// <summary>
        /// Левый верхний угол поля
        /// </summary>
        public Point LeftTopPoint;
        /// <summary>
        /// Правый нижний угол поля
        /// </summary>
        public Point RightBottonPoint;
        /// <summary>
        /// Игровые объекты
        /// </summary>
        public List<GameObject> GameObjects { get; private set; }
        /// <summary>
        /// Игровая анимация (взрывы, текстовая анимация и т.д.)
        /// </summary>
        public ICollection<GameAnimation> Animations { get; private set; }
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public GameData(int cellSize, int xCellCount, int yCellCount, int indention)
        {
            XCellCount = xCellCount;
            YCellCount = yCellCount;
            LeftTopPoint = new Point(indention, indention);
            RightBottonPoint = new Point(indention + xCellCount * cellSize, indention + yCellCount * cellSize);

            GameObjects = new List<GameObject>();
            Animations = new List<GameAnimation>();
        }
    }
}
