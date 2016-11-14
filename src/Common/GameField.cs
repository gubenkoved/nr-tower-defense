using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NRTowerDefense
{

    public enum BlockTestResult
    {
        /// <summary>
        /// Найдены блокировки на пути из стартовой вершины к целевой
        /// </summary>
        StartToTargetBlock,
        /// <summary>
        /// Блокировка активных монстров
        /// </summary>
        MonsterBlock,
        /// <summary>
        /// Блокировок не найдено
        /// </summary>
        NoBlock
    }

    public static class HelperExtensions
    {
        /// <summary>
        /// Возвращет либо ядро декораторов, если GameObject это декоратор, иначе вернёт переданный GameObject
        /// </summary>
        public static GameObject RealObject(this GameObject obj)
        {
            if (obj is GameObjectDecorator)
                return (obj as GameObjectDecorator).Kernel;
            else
                return obj;
        }

        public static GameCell Length(this List<Point> path)
        {
            double resultInPx = 0.0;

            for (int i = 1; i < path.Count; i++)
            {
                resultInPx += Helper.HelperFunctions.GetLength(path[i], path[i - 1]);
            }

            return new GameCell(resultInPx / (double)Game.Field.CellSize);
        }
    }

    public class GameField
    {
        /// <summary>
        /// Размер ячейки
        /// </summary>
        public readonly int CellSize = GameProperties.GameFieldCellSize;
        /// <summary>
        /// Отступ реального игрового пространства от границ канваса
        /// </summary>
        public readonly int Indention = GameProperties.GameFieldIndention;
        /// <summary>
        /// Стартовая позиция монстров
        /// </summary>
        public Point StartMonsterPosition { get; private set; }
        /// <summary>
        /// Целевая позиция монсторов
        /// </summary>
        public Point TargetMonsterPosition { get; private set; }
        /// <summary>
        /// Разброс монстров от стартовой позиции
        /// </summary>
        public readonly double PositionDispersion = GameProperties.GameFieldStartPositionDispersion;
        /// <summary>
        /// Связанный канвас
        /// </summary>
        public Canvas ConnectedCanvas { get; private set; }
        /// <summary>
        /// Инкапсулирует все игровые объекты на поле
        /// </summary>
        private GameData gameData;
        /// <summary>
        /// Путь из начала карты в конец
        /// </summary>
        public List<Point> StartToEndPath;
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public GameField(Canvas connectedCanvas)
        {
            int XCellCount = (int) Math.Floor((connectedCanvas.ActualWidth - 2 * Indention) / CellSize);
            int YCellCount = (int) Math.Floor((connectedCanvas.ActualHeight - 2 * Indention) / CellSize);
            
            ConnectedCanvas = connectedCanvas;
            gameData = new GameData(CellSize, XCellCount, YCellCount, Indention);

            StartMonsterPosition = new Point(Indention + ((XCellCount / 2) + 0.5) * CellSize, Indention + 0.5 * CellSize);
            TargetMonsterPosition = new Point(Indention + ((XCellCount / 2) + 0.5) * CellSize, Indention + (YCellCount - 0.5) * CellSize);

            StartToEndPath = null;
        }
        /// <summary>
        /// Действия по истечению тика
        /// </summary>
        public void Tick(double interval)
        {
            // обновляем в первый раз, когда StartToEndPath ещё не было вычисленно ни разу
            if (StartToEndPath == null)
                UpdateStartToEndPath();

            gameData.GameObjects.ForEach(o => o.Tick(interval));

            DeleteOldGameObjects();

            Draw();
        }
        /// <summary>
        /// Отрисовываем всё игровое поле на канвас
        /// </summary>
        public void Draw()
        {
            if (!fieldCreated)
                drawField();

            gameData.GameObjects.ForEach(o => o.Draw(ConnectedCanvas));
        }
        /// <summary>
        /// Добавляем башню на поле
        /// </summary>
        public void AddTower(Tower tower)
        {
            // вешаем декораторы
            gameData.GameObjects.Add(new BorderGameObjectDecorator(new ReloadingIndicatorTowerDecorator(new LevelTowerDecorator(tower))));

            Game.UpdateInformationControl(Game.ControlType.MoneyControl);
        }
        /// <summary>
        /// Удаление башни
        /// </summary>
        public void RemoveTower(Tower tower)
        {
            GameObject obj = findCommonGameObject(tower);
            gameData.GameObjects.Remove(obj);

            pathCache.Clear();
            RecalculateAllMonsterPaths();            
        }
        /// <summary>
        /// Продать башню
        /// </summary>
        public void SellTower(Tower tower)
        {
            double moneyInc = GetSellCost(tower);

            findCommonGameObject(tower).ClearFromCanvas(ConnectedCanvas);

            RemoveTower(tower);

            Game.ChangeMoney( moneyInc );
            Game.UpdateInformationControl(Game.ControlType.MoneyControl);

            #region Добавляем анимацию продажи башни
            AddAnimation(
                        new TextGameAnimation
                        (
                            moneyInc.ToString("+#"),
                            tower.Position,
                            Colors.Gold,
                            4.0,
                            18
                        ));

            AddAnimation(
                    new DisappearanceImageGameAnimation(
                            new Uri("/images/moneybag.png", UriKind.Relative),
                            tower.Position,
                            10.0,
                            Game.Field.CellSize
                         )
                ); 
            #endregion
        }
        /// <summary>
        /// Расчитывает цену для продажи башни
        /// </summary>
        public double GetSellCost(Tower tower)
        {
            return tower.Cost * 0.5;
        }
        /// <summary>
        /// Генератор рандома
        /// </summary>
        private Random random = new Random();
        /// <summary>
        /// Возвращает все башни
        /// </summary>
        public List<Tower> GetTowers()
        {
            List<Tower> result = new List<Tower>();
            
            foreach (GameObject obj in gameData.GameObjects)
            {
                if (obj.RealObject() is Tower)
                    result.Add(obj.RealObject() as Tower);
            }
            
            return result;
        }
        /// <summary>
        /// Возвращает всех монстров
        /// </summary>
        public List<Monster> GetMonsters()
        {
            List<Monster> result = new List<Monster>();

            foreach (GameObject obj in gameData.GameObjects)
            {
                if (obj.RealObject() is Monster)
                    result.Add(obj.RealObject() as Monster);
            }

            return result;
        }
        /// <summary>
        /// Возвращает все ракеты
        /// </summary>
        public List<Bullet> GetBullets()
        {
            List<Bullet> result = new List<Bullet>();

            foreach (GameObject obj in gameData.GameObjects)
            {
                if (obj.RealObject() is Bullet)
                    result.Add(obj.RealObject() as Bullet);
            }

            return result;
        }
        /// <summary>
        /// Возвращает количество клеток по OX
        /// </summary>
        /// <returns></returns>
        public int GetXCellCount()
        {
            return gameData.XCellCount;
        }
        /// <summary>
        /// Возвращает количество клеток по OY
        /// </summary>
        /// <returns></returns>
        public int GetYCellCount()
        {
            return gameData.YCellCount;
        }
        /// <summary>
        /// Добавляем монстра на поле
        /// </summary>
        /// <param name="monster"></param>
        public void AddMonster(Monster monster)
        {
            monster.Position = new Point(StartMonsterPosition.X + (random.NextDouble() - 0.5) * PositionDispersion,StartMonsterPosition.Y + (random.NextDouble() - 0.5) * PositionDispersion);

            // вешаем декораторы
            gameData.GameObjects.Add(new BorderGameObjectDecorator(new LifeIndicatorMonsterDecorator( monster )));

            List<Point> pathFromCache = pathCache.TryGetPath(monster.Position);
            if (pathFromCache != null)
            {
                monster.SetPath( pathFromCache );
            }
            else
            {
                monster.RecalculatePath();
                pathCache.AddPath(monster.GetPath());
            }

            ++TotalMonsterCounter;
        }
        /// <summary>
        /// Добавляет ракету на поле
        /// </summary>
        /// <param name="bullet"></param>
        public void AddBullet(Bullet bullet)
        {
            //System.Media.SoundPlayer sp = new System.Media.SoundPlayer();
            //sp.SoundLocation = @"C:\Windows\Media\tada.wav";
            //sp.Play();
            gameData.GameObjects.Add(bullet);
        }
        /// <summary>
        /// Добавляет анимацию на поле
        /// </summary>
        /// <param name="bullet"></param>
        public void AddAnimation(GameAnimation animation)
        {
            gameData.Animations.Add(animation);
            animation.StartAnimation(ConnectedCanvas);
        }
        /// <summary>
        /// Имеется ли прототип башни, который может быть построен
        /// </summary>
        /// <returns></returns>
        public bool HasBuildablePrototype()
        {
            foreach (Tower tower in GetTowers())
            {
                if (tower is TowerPrototype && (tower as TowerPrototype).CanBuild)
                    return true;
            }

            return false;
        }
        /// <summary>
        /// Имеется ли прототип башен
        /// </summary>
        /// <returns></returns>
        public bool HasPrototype()
        {
            foreach (Tower tower in GetTowers())
            {
                if (tower is TowerPrototype)
                    return true;
            }

            return false;
        }
        /// <summary>
        /// Построить башню на месте прототипа
        /// </summary>
        public BlockTestResult BuildPrototype()
        {
            foreach (GameObject obj in gameData.GameObjects)
            {
                if (obj.RealObject() is TowerPrototype)
                {
                    var prototype = obj.RealObject() as TowerPrototype;

                    #region Проверяем не блокируются ли пути монстров, для этого временно попытаемя посторить прототип
                    BlockTestResult blockTestResult;
                    
                    gameData.GameObjects.Add(prototype.TargetTower);
                    pathCache.ClearInvalidPaths(prototype.TargetTower);
                    blockTestResult = BlockTest();
                    gameData.GameObjects.Remove(prototype.TargetTower); 
                    #endregion

                    if (blockTestResult == BlockTestResult.NoBlock)
                    {
                        prototype.Build();
                        RecalculateAllMonsterPaths();                        
                        break;
                    }
                    else
                    {
                        Game.OnBlockDetected(blockTestResult);
                        return blockTestResult;
                    }
                }
            }

            return BlockTestResult.NoBlock;
        }
        /// <summary>
        /// Удаляет с поля прототип башни
        /// </summary>
        public void ClearPrototype()
        {
            foreach (GameObject obj in gameData.GameObjects)
            {
                if (obj.RealObject() is TowerPrototype)
                {
                    var prototype = obj.RealObject() as TowerPrototype;

                    obj.ClearFromCanvas(ConnectedCanvas);
                    gameData.GameObjects.Remove(obj);

                    break;
                }
            }
        }
        /// <summary>
        /// Пытается для заданного obj найти "самый верхний" объемлющий его декоратор, иначе возвращает указанный GameObject
        /// </summary>
        private GameObject findCommonGameObject(GameObject findingGameObject)
        {
            foreach (GameObject gameObject in gameData.GameObjects)
            {
                if (gameObject.RealObject() == findingGameObject)
                    return gameObject;
            }

            throw new Exception("Искомый игровой объект должен быть в коллекции игровых объектов!");
        }
        /// <summary>
        /// Для заданного GameObject ищет декоратор по заданному типу
        /// </summary>
        private GameObjectDecorator findDecorator(GameObject obj, Type decoratorType)
        {
            GameObject active = obj;

            while (active is GameObjectDecorator)
            {
                GameObjectDecorator activeDecorator = active as GameObjectDecorator;

                if (activeDecorator.GetType() == decoratorType)
                    return activeDecorator;
            }

            return null;
        }
        /// <summary>
        /// Пометить объект как выделенный
        /// </summary>
        public void SetAsSelected(GameObject obj)
        {
            #region Приводим к общим классам игровых объектов, и снимаем выделение
            if (obj is Tower)
                DeselectAll(typeof(Tower));
            else if (obj is Monster)
                DeselectAll(typeof(Monster));
            else if (obj is Bullet)
                DeselectAll(typeof(Bullet));
            #endregion

            obj.State = GameObjectState.Selected;

            BorderGameObjectDecorator borderDecorator = findDecorator(findCommonGameObject(obj), typeof(BorderGameObjectDecorator)) as BorderGameObjectDecorator;
            if (borderDecorator != null)
                borderDecorator.Enabled = true;

            #region Вызываем события из класса Game
            if (obj is Monster)
                Game.OnMonsterSelected(obj as Monster);
            else if (obj is Tower)
                Game.OnTowerSelected(obj as Tower); 
            #endregion
        }
        /// <summary>
        /// Снимает выделение со всех объектов заданного типа
        /// </summary>
        public void DeselectAll(Type type)
        {
            foreach (GameObject obj in gameData.GameObjects)
            {
                if (type.IsInstanceOfType(obj.RealObject()))
                {
                    obj.RealObject().State = GameObjectState.Simple;

                    BorderGameObjectDecorator borderDecorator = findDecorator(obj, typeof(BorderGameObjectDecorator)) as BorderGameObjectDecorator;
                    
                    if (borderDecorator != null)
                        borderDecorator.Enabled = false;
                }
            }
        }
        /// <summary>
        /// Возвращает выделенный объект заданного типа
        /// </summary>
        public GameObject GetSelectedObject(Type type)
        {
            foreach (GameObject obj in gameData.GameObjects)
            {
                if (obj.RealObject().State == GameObjectState.Selected && type.IsInstanceOfType(obj.RealObject()))
                    return obj.RealObject();
            }

            return null;
        }
        /// <summary>
        /// Кэш найденных маршрутов
        /// </summary>
        private PathCache pathCache = new PathCache();
        /// <summary>
        /// Тест на наличеие блокировок на поле
        /// </summary>
        /// <returns></returns>
        public BlockTestResult BlockTest()
        {
            if (PathFinder.FindPath(StartMonsterPosition,TargetMonsterPosition) == null)
            {
                return BlockTestResult.StartToTargetBlock;
            }
            else
            {
                List<Monster> monsters = GetMonsters();
                for (int i = monsters.Count - 1; i >= 0; i--)
                {
                    Monster monster = monsters[i];

                    List<Point> pathFromCache = pathCache.TryGetPath(monster.Position);

                    if (pathFromCache == null)
                    {
                        List<Point> monsterPath = PathFinder.FindPath(monster.Position, TargetMonsterPosition);
                        if (monsterPath == null)
                        {
                            return BlockTestResult.MonsterBlock;
                        }
                        else
                        {
                            pathCache.AddPath(monsterPath);
                        }
                    }
                }
            }

            return BlockTestResult.NoBlock;
        }
        /// <summary>
        /// Обновляет маршрут из начала игрового поля в конец
        /// </summary>
        /// <returns></returns>
        public void UpdateStartToEndPath()
        {
            StartToEndPath = PathFinder.FindPath(StartMonsterPosition, TargetMonsterPosition);

            redrawStartToEndPath();

            Game.UpdateInformationControl(Game.ControlType.StartEndPathLenControl);
        }
        /// <summary>
        /// Пересчет путей монстров
        /// </summary>
        public void RecalculateAllMonsterPaths(bool clearCache = false)
        {
            if (clearCache)
                pathCache.Clear();

            UpdateStartToEndPath();

            List<Monster> monsters = GetMonsters();

            for (int i = monsters.Count - 1; i >= 0; i--)
			{
                Monster monster = monsters[i];

                List<Point> pathFromCache = pathCache.TryGetPath(monster.Position);

                if (pathFromCache != null)
                {
                    monster.SetPath(pathFromCache);
                }
                else
                {
                    monster.RecalculatePath();
                    pathCache.AddPath(monster.GetPath());
                }
            }
        }
        /// <summary>
        /// Удаляем устаревшие объекты
        /// </summary>
        public void DeleteOldGameObjects()
        {
            #region Delete old game objects
            List<GameObject> oldObjects = new List<GameObject>();

            #region died monsters
            foreach (GameObject obj in gameData.GameObjects)
            {
                Monster monster = obj.RealObject() as Monster;
                
                if (monster != null && monster.Killed)
                    oldObjects.Add(obj);
            }
            #endregion

            #region hitted bullets
            foreach (GameObject obj in gameData.GameObjects)
            {
                Bullet bullet = obj.RealObject() as Bullet;

                if (bullet != null && bullet.Hit)
                    oldObjects.Add(obj);
            }
            #endregion

            #region exploded towers
            foreach (GameObject obj in gameData.GameObjects)
            {
                Tower tower = obj.RealObject() as Tower;

                if (tower != null && tower is IExplodeTower && (tower as IExplodeTower).Exploded)
                    oldObjects.Add(obj);
            }
            #endregion

            foreach (GameObject oldObject in oldObjects)
            {
                oldObject.ClearFromCanvas(ConnectedCanvas);
                gameData.GameObjects.Remove(oldObject);
            }
            #endregion

            #region Delete finished animations
            List<GameAnimation> finishedAnamations = new List<GameAnimation>();
            foreach (GameAnimation item in gameData.Animations)
            {
                if (item.Finished)
                {
                    item.ClearFromCanvas(ConnectedCanvas);
                    finishedAnamations.Add(item);
                }
            }
            foreach (GameAnimation item in finishedAnamations)
            {
                gameData.Animations.Remove(item);
            }
            #endregion
        }
        /// <summary>
        /// Возвращает ракеты, летящие к заданному монстру
        /// </summary>
        public List<Bullet> GetBulletsWithTarget(Monster monster)
        {
            List<Bullet> bulletsWithGivenTarget = new List<Bullet>();

            foreach (Bullet bullet in GetBullets())
            {
                if (bullet.Target == monster)
                {
                    bulletsWithGivenTarget.Add(bullet);
                }
            }

            return bulletsWithGivenTarget;
        }
        /// <summary>
        /// Подсчитывает возможный урон от заданных ракет
        /// </summary>
        public double CalculateSummaryDamage(Monster monster, List<Bullet> bullets)
        {
            double summaryDamage = 0.0;

            foreach (Bullet bullet in bullets)
            {
                summaryDamage += monster.CalculateDamage(bullet);
            }

            return summaryDamage;
        }
        /// <summary>
        /// Счётчик общего числа добавленных монстров
        /// </summary>
        public int TotalMonsterCounter;
        /// <summary>
        /// Возвращает ZIndex для последнего монстра, опираясь на TotalMonsterCounter
        /// </summary>
        public int GetLastMonsterZIndex()
        {
            return GameProperties.MaxMonsterZIndex - TotalMonsterCounter;
        }
        /// <summary>
        /// Производит очистку поля, и подготовливает его для новой игры
        /// </summary>
        public void ReCreate()
        {
            foreach (var obj in gameData.GameObjects)
            {
                obj.ClearFromCanvas(ConnectedCanvas);
            }

            gameData.GameObjects.Clear();
            gameData.Animations.Clear();

            UpdateStartToEndPath();

            pathCache.Clear();
        }
        #region DRAWING FIELD
        private bool fieldCreated = false;
        private List<UIElement> fieldUIElements = new List<UIElement>();
        private int pointRadius = 5;
        private void drawField()
        {
            Brush fieldBrush = new SolidColorBrush(Color.FromArgb(80, 0, 0, 0));
            double fieldLineThickness = 1.0;

            if (!fieldCreated)
            {
                #region Lines
                #region Vertical lines
                for (int i = 0; i <= gameData.XCellCount; i++)
                {
                    Line line = new Line();
                    line.X1 = Indention + i * CellSize;
                    line.Y1 = Indention;
                    line.X2 = Indention + i * CellSize;
                    line.Y2 = Indention + gameData.YCellCount * CellSize;

                    line.StrokeStartLineCap = line.StrokeEndLineCap = PenLineCap.Round;
                    line.Stroke = fieldBrush;
                    line.StrokeThickness = fieldLineThickness;
                    line.SnapsToDevicePixels = true;

                    Panel.SetZIndex(line, GameProperties.GameFieldGridZIndex);

                    fieldUIElements.Add(line);
                }
                #endregion

                #region Horisontal lines
                for (int j = 0; j <= gameData.YCellCount; j++)
                {
                    Line line = new Line();
                    line.X1 = Indention;
                    line.Y1 = Indention + j * CellSize;
                    line.X2 = Indention + gameData.XCellCount * CellSize;
                    line.Y2 = Indention + j * CellSize;

                    line.StrokeStartLineCap = line.StrokeEndLineCap = PenLineCap.Round;
                    line.Stroke = fieldBrush;
                    line.StrokeThickness = fieldLineThickness;
                    line.SnapsToDevicePixels = true;

                    Panel.SetZIndex(line, GameProperties.GameFieldGridZIndex);

                    fieldUIElements.Add(line);
                }
                #endregion 
                #endregion

                #region Start and target points
                #region Start point
                Image startPointImg = new Image();
                startPointImg.Source = new BitmapImage(new Uri("/images/circle_green.png", UriKind.Relative));

                startPointImg.Width = startPointImg.Height = 2 * pointRadius;

                Panel.SetZIndex(startPointImg, GameProperties.GameFieldStartEndPointZIndex);
                Canvas.SetLeft(startPointImg, StartMonsterPosition.X - pointRadius);
                Canvas.SetTop(startPointImg, StartMonsterPosition.Y - pointRadius);

                fieldUIElements.Add(startPointImg);
                #endregion
                #region Target point
                Image targetPointImg = new Image();
                targetPointImg.Source = new BitmapImage(new Uri("/images/circle_red.png", UriKind.Relative));

                targetPointImg.Width = targetPointImg.Height = 2 * pointRadius;

                Panel.SetZIndex(targetPointImg, GameProperties.GameFieldStartEndPointZIndex);
                Canvas.SetLeft(targetPointImg, TargetMonsterPosition.X - pointRadius);
                Canvas.SetTop(targetPointImg, TargetMonsterPosition.Y - pointRadius);

                fieldUIElements.Add(targetPointImg);
                #endregion
                #endregion

                foreach (UIElement item in fieldUIElements)
	            {
                    
		            ConnectedCanvas.Children.Add(item);
	            }
                
                fieldCreated = true;
            }
        }
        private Path startToEndDrawingPath;
        private void redrawStartToEndPath()
        {
            if (startToEndDrawingPath == null)
            {
                startToEndDrawingPath = new Path();

                startToEndDrawingPath.StrokeThickness = 5;
                startToEndDrawingPath.Stroke = new SolidColorBrush(Color.FromArgb(50, 0, 255, 0));
                startToEndDrawingPath.StrokeStartLineCap = startToEndDrawingPath.StrokeEndLineCap = PenLineCap.Round;

                Panel.SetZIndex(startToEndDrawingPath, GameProperties.GameFieldStartToEndPathZIndex);
                ConnectedCanvas.Children.Add(startToEndDrawingPath);
            }

            GeometryGroup pathLines = new GeometryGroup();
            startToEndDrawingPath.Data = pathLines;

            for (int i = 1; i < StartToEndPath.Count; i++)
            {
                pathLines.Children.Add(new LineGeometry(StartToEndPath[i - 1], StartToEndPath[i]));
            }
        }
        #endregion
    }
}
