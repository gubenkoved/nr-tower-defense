using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Helper;

using System.IO;    // SPEED TEST

namespace NRTowerDefense
{
    using PathType = List<Point>;    

    static public class PathFinder
    {

        internal class Node
        {
            public Point Position;
            public double Weight;
            public bool Visited;
            public Node Parent;
            public Node(Point position)
            {
                Position = position;
                Weight = double.MaxValue;
                Visited = false;
                Parent = null;
            }
        }

        static internal List<Node> getIncedentUnvisitedNodes(List<Node> graph, Node current)
        {
            List<Node> result = new List<Node>();

            foreach (Node node in graph)
            {
                if (HelperFunctions.GetLength(node.Position, current.Position) <= Math.Sqrt(2) * Game.Field.CellSize && node.Visited == false)
                {
                    result.Add(node);    
                }
            }

            return result;
        }

        static internal Node getUnvisitedNodeWithMinWeight(List<Node> graph)
        {
            Node result = null;

            foreach (Node node in graph)
            {
                if (!node.Visited && (result == null || node.Weight < result.Weight))
                    result = node;
            }

            return result;
        }

        static internal bool isUnitBusy(Point unit)
        {
            foreach (Tower tower in Game.Field.GetTowers())
            {
                if (tower is BombTower && (tower as BombTower).Exploded)
                    continue;

                if (HelperFunctions.GetLength(tower.Position, unit) <= tower.Radius.ToPixels())
                {
                    return true;
                }
            }
            return false;
        }        

        /// <summary>
        /// Поиск пути из точки в точку
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <param name="targetPosition"></param>
        static public PathType FindPath(Point currentPosition, Point targetPosition)
        {
            #region Speed test
            //DateTime time = DateTime.Now;
            //StreamWriter sw = new StreamWriter("finder.txt", true); 
            #endregion

            PathType path = new PathType();

            #region Формируем граф в котором будем искать кратчайщие расстояния
            List<Node> graph = new List<Node>();
            graph.Add(new Node(currentPosition));
            graph.Add(new Node(targetPosition));

            #region Не заняты ли начальная и конечная точки
            if (isUnitBusy(currentPosition) || isUnitBusy(targetPosition))
                return null;
            #endregion

            for (int i = 1; i <= Game.Field.GetXCellCount(); i++)
            {
                for (int j = 1; j <= Game.Field.GetYCellCount(); j++)
                {
                    Point unit = new Point(Game.Field.Indention + (i - 0.5) * Game.Field.CellSize, Game.Field.Indention + (j - 0.5) * Game.Field.CellSize);

                    if (!isUnitBusy(unit))
                        graph.Add(new Node(unit));
                }
            } 
            #endregion

            #region Прогоняем алгоритм Дейкстры
            graph[0].Weight = 0.0;

            Node activeNode;
            List<Node> incedentUnvisited;

            while ((activeNode = getUnvisitedNodeWithMinWeight(graph)) != null)
            {
                activeNode.Visited = true;
                incedentUnvisited = getIncedentUnvisitedNodes(graph, activeNode);

                foreach (Node node in incedentUnvisited)
                {
                    if (activeNode.Weight + HelperFunctions.GetLength(activeNode.Position, node.Position) < node.Weight)
                    {
                        node.Weight = activeNode.Weight + HelperFunctions.GetLength(activeNode.Position, node.Position);
                        node.Parent = activeNode;
                    }
                }
            }

            #region Формируем результат
            activeNode = graph[1];
            while (activeNode != null)
            {
                path.Add(activeNode.Position);
                activeNode = activeNode.Parent;
            }

            path.Reverse(); 
            #endregion 
            #endregion

            #region Speed test
            //sw.WriteLine((DateTime.Now - time).TotalMilliseconds.ToString("F2"));
            //sw.Close();
            #endregion

            if (graph[1].Weight == double.MaxValue)
                return null;
            else
                return path;
        }
    }
}
