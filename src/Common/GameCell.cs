using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRTowerDefense
{
    public struct GameCell
    {
        /// <summary>
        /// Множитель
        /// </summary>
        public double Amount;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="amount"></param>
        public GameCell(double amount)
        {
            Amount = amount;
        }
        /// <summary>
        /// Преобразовать в пикселы
        /// </summary>
        /// <returns></returns>
        public double ToPixels()
        {
            return Game.Field.CellSize * Amount;
        }
        /// <summary>
        /// Явное преобразование в пикселы
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        //public static implicit operator double(GameCell c)
        //{
        //    return c.ToPixels();
        //}
        /// <summary>
        /// Операция сложения
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static GameCell operator + (GameCell c1, GameCell c2 )
        {
            return new GameCell( c1.Amount + c2.Amount );
        }
        /// <summary>
        /// Операция вычитания
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static GameCell operator -(GameCell c1, GameCell c2)
        {
            if (double.IsPositiveInfinity(c1.Amount) && double.IsPositiveInfinity(c2.Amount))
                return new GameCell(0);
            else
                return new GameCell(c1.Amount - c2.Amount);
        }        
    }
}
