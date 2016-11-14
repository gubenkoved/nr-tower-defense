using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Helper
{
    /// <summary>
    /// Класс содержит вспомогательные статические функции
    /// </summary>
    static public class HelperFunctions
    {
        /// <summary>
        /// Возвращает угол наклона прямой заданной двумя точками и прямой направленной по оси OY, учитывая перевёрнутость системы координат
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static double GetAngleFromPointToPoint(Point first, Point second)
        {
            Vector vector = new Vector(second.X - first.X, -1 * (second.Y - first.Y));    // *-1 т.к. перевернутая система координат
            Vector upVector = new Vector(0.0, 1.0);
            return Vector.AngleBetween(vector, upVector);
        }
        public static double GetLength(Point first, Point second)
        {
            return Math.Sqrt((second.X - first.X) * (second.X - first.X)
                + (second.Y - first.Y) * (second.Y - first.Y));
        }
        public static bool PointInLine(Point lineBegin, Point lineEnd, Point point)
        {
            double dergeesEps = 1.0;
            double x1 = lineBegin.X;
            double x2 = lineEnd.X;
            double y1 = lineBegin.Y;
            double y2 = lineEnd.Y;

            if (x2 < x1)
            {
                double t = x1;
                x1 = x2;
                x2 = t;
            }
            if (y2 < y1)
            {
                double t = y1;
                y1 = y2;
                y2 = t;
            }

            if (point.X >= x1 && point.X <= x2 && point.Y >= y1 && point.Y <= y2)
            {
                if (GetAngleFromPointToPoint(lineBegin, lineEnd) - GetAngleFromPointToPoint(point, lineEnd) < dergeesEps)
                    return true;
            }
            return false;
        }
    }

}