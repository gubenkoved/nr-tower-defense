using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Helper;

namespace NRTowerDefense
{
    using Path = List<Point>;
    using Paths = List<List<Point>>;

    // Класс, для оптимизации производительности поиска путей
    public class PathCache
    {
        private uint cacheHit;
        private uint cacheMiss;
        private Paths findedPaths = new Paths();
        /// <summary>
        /// Добавляет валидный путь в кэш
        /// </summary>
        /// <param name="path"></param>
        public void AddPath(Path path)
        {
            findedPaths.Add(path);
        }
        /// <summary>
        /// Полностью очищает кэш
        /// </summary>
        public void Clear()
        {
            findedPaths.Clear();
        }
        /// <summary>
        /// Попытка получить из кэша путь, для монстра находящегося в заданной точке
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Path TryGetPath(Point point)
        {
            Path result = tryGetPathImpl(point);

            if (result == null)
                ++cacheMiss;
            else
                ++cacheHit;

            return result;
        }
        /// <summary>
        /// Реализация основной логики кэша
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private Path tryGetPathImpl(Point point)
        {
            Path result = null;

            foreach (Path path in findedPaths)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    if (HelperFunctions.PointInLine(path[i], path[i + 1], point))
                    {
                        return truncatePath(path, i + 1);
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Формирует обрезанный путь, начиная с k-й точки до конца
        /// </summary>
        private Path truncatePath(Path path, int k)
        {
            Path result = new Path();

            for (int i = k; i < path.Count; i++)
            {
                result.Add(path[i]);
            }

            return result;
        }
        /// <summary>
        /// Удаляет недействительные пути, которые могли появиться из-за установки башни
        /// </summary>
        public void ClearInvalidPaths(Tower tower)
        {
            Paths invalidPaths = new Paths();

            foreach (Path path in findedPaths)
            {
                foreach (Point point in path)
                {
                    if (HelperFunctions.GetLength(point, tower.Position) < tower.Radius.ToPixels())
                    {
                        invalidPaths.Add(path);
                        break;
                    }
                }
            }

            foreach (Path path in invalidPaths)
            {
                findedPaths.Remove(path);
            }
        }
    }
}
