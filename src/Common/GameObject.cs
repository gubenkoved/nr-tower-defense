using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace NRTowerDefense
{
    public abstract class GameObject: ITickable, IDrawable
    {
        /// <summary>
        /// Статус игрового объекта
        /// </summary>
        public GameObjectState State { set; get;}
        /// <summary>
        /// Радиус игрового объекта
        /// </summary>
        public GameCell Radius { set; get; }
        /// <summary>
        /// Позиция игрового объекта на поле
        /// </summary>
        public Point Position { set; get; }

        /// <summary>
        /// Реакция на тик игрового объекта
        /// </summary>
        /// <param name="interval"></param>
        public abstract void Tick(double interval);

        /// <summary>
        /// Отрисовка игрового объекта
        /// </summary>
        /// <param name="canvas"></param>
        public abstract void Draw(Canvas canvas);
        /// <summary>
        /// Удаление игрового объекта с канваса
        /// </summary>
        /// <param name="canvas"></param>
        public abstract void ClearFromCanvas(Canvas canvas);
    }
}
