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

namespace NRTowerDefense
{

    public class TowerArg
    {
        public Tower Tower { get; private set; }
        public TowerArg(Tower tower)
        {
            Tower = tower;
        }
    }

    public class MonsterArg
    {
        public Monster Monster { get; private set; }
        public MonsterArg(Monster monster)
        {
            Monster = monster;
        }
    }

    public class BlockDetectedArg
    {
        public BlockTestResult BlockTestResult { get; private set; }
        public BlockDetectedArg(BlockTestResult blockTestResult)
        {
            BlockTestResult = blockTestResult;
        }
    }

    /// <summary>
    /// Класс игра
    /// </summary>
    static public class Game
    {
        /// <summary>
        /// Типы иформационных контролов
        /// </summary>
        public enum ControlType
        {
            LifeControl, MoneyControl, TimeControl, ScoreControl , StartEndPathLenControl
        }
        /// <summary>
        /// Перечисление причин остановки игры
        /// </summary>
        public enum GameStatus
        {
            NotStarted, Active, PlayerLose, PlayerWin
        }
        /// <summary>
        /// Игровое поле
        /// </summary>
        static public GameField Field { get; private set; }
        /// <summary>
        /// Словарик из контролов на которые будет выводиться информация по игре ( время, деньги, жизни ... )
        /// </summary>
        static private Dictionary<ControlType, ContentControl> informationControls = new Dictionary<ControlType, ContentControl>();
        /// <summary>
        /// Количество денег
        /// </summary>
        static public double Money { get; private set; }
        /// <summary>
        /// Изменение количества денег
        /// </summary>
        static public void ChangeMoney(double delta)
        {
            Money += delta;

            if (delta > 0)
            {
                Field.AddAnimation(new ColorFieldGameAnimation(Colors.Green, 0.5, 0.15, 0.0));
            }
        }
        /// <summary>
        /// Количество жизней
        /// </summary>
        static public int Life { get; private set; }
        /// <summary>
        /// Игровые очки
        /// </summary>
        static public double Score { get; private set; }
        /// <summary>
        /// Таймер игры
        /// </summary>
        static public GameTimer Timer { get; private set; }
        /// <summary>
        /// Причина остановки игры
        /// </summary>
        static public GameStatus Status { get; private set; }
        /// <summary>
        /// Текущий игровой уровень
        /// </summary>
        static public GameLevel CurrentLevel { get; private set; }
        /// <summary>
        /// Инициализация игры
        /// </summary>
        static public void Init(Canvas gameCanvas)
        {
            Status = GameStatus.NotStarted;

            Field = new GameField(gameCanvas);

            Timer = new GameTimer((int)(1000.0 / GameProperties.RequireFPS));
            Timer.Tick += (s, e) => Field.Tick(e.Interval); // подписываем GameField на тик таймера
            Timer.Pause();
        }
        /// <summary>
        /// Новая игра
        /// </summary>
        static public void NewGame(GameLevel gameLevel)
        {
            Money = GameProperties.StartMoney;
            Life = GameProperties.StartLife;
            Score = GameProperties.StartScore;
            Status = GameStatus.Active;

            UpdateInformationControls();

            Field.ReCreate();

            Timer.Reset();
            Timer.Unpause();

            CurrentLevel = gameLevel;
            CurrentLevel.Completed += new GameLevel.LevelHandler((l) =>
                {
                    if (CurrentLevelCompeted != null)
                        CurrentLevelCompeted(l);

                    Status = GameStatus.PlayerWin;
                    Timer.Pause();
                });

            if (GameStarted != null)
                GameStarted();
        }
        /// <summary>
        /// Обновляет словарик контролов
        /// </summary>
        static public void SetInformationControls(ContentControl lifeControl, ContentControl moneyControl, ContentControl scoreControl, ContentControl timeControl, ContentControl startEndPathLenControl)
        {
            if (lifeControl != null)
                informationControls.Add(ControlType.LifeControl, lifeControl);
            if (moneyControl != null)
                informationControls.Add(ControlType.MoneyControl, moneyControl);
            if (timeControl != null)
                informationControls.Add(ControlType.TimeControl, timeControl);
            if (startEndPathLenControl != null)
                informationControls.Add(ControlType.StartEndPathLenControl, startEndPathLenControl);
            if (scoreControl != null)
                informationControls.Add(ControlType.ScoreControl, scoreControl);
        }
        /// <summary>
        /// Обновляет информацию на свзанных контролах
        /// </summary>
        static public void UpdateInformationControls()
        {
            UpdateInformationControl(ControlType.LifeControl);
            UpdateInformationControl(ControlType.MoneyControl);
            UpdateInformationControl(ControlType.TimeControl);
            UpdateInformationControl(ControlType.StartEndPathLenControl);
            UpdateInformationControl(ControlType.ScoreControl);
        }
        /// <summary>
        /// Обновляет информацию о конкретном параметре
        /// </summary>
        static public void UpdateInformationControl( ControlType type )
        {
            switch (type)
            {
                case ControlType.LifeControl:
                    if (informationControls.ContainsKey(ControlType.LifeControl))
                        informationControls[ControlType.LifeControl].Content = Life.ToString();
                    break;
                case ControlType.MoneyControl:
                    if (informationControls.ContainsKey(ControlType.MoneyControl))
                        informationControls[ControlType.MoneyControl].Content = Money.ToString();
                    break;
                case ControlType.TimeControl:
                    if (informationControls.ContainsKey(ControlType.TimeControl))
                        informationControls[ControlType.TimeControl].Content = Timer.GetFormatedTime();
                    break;
                case ControlType.StartEndPathLenControl:
                    if (informationControls.ContainsKey(ControlType.StartEndPathLenControl))
                        if (Field.StartToEndPath != null)
                            informationControls[ControlType.StartEndPathLenControl].Content = Field.StartToEndPath.Length().Amount.ToString("#.#×");
                    break;
                case ControlType.ScoreControl:
                    if (informationControls.ContainsKey(ControlType.ScoreControl))
                        informationControls[ControlType.ScoreControl].Content = Score;
                    break;
            }
        }
        /// <summary>
        /// Вызывается при прохождении монстра к пункту назначения
        /// </summary>
        static public void OnMonsterPass(Monster monster)
        {
            Field.AddAnimation(new ColorFieldGameAnimation(Colors.Red, 2.0, 0.15, 0.0));

            Life -= 1;

            if (Life == 0)
            {
                Status = GameStatus.PlayerLose;

                Timer.Pause();

                if (GameOver != null)
                    GameOver(new LevelArg(CurrentLevel));
            }

            UpdateInformationControl(ControlType.LifeControl);

            if (MonsterPass != null)
                MonsterPass(new MonsterArg(monster));

            if (MonsterGone != null)
                MonsterGone(new MonsterArg(monster));
        }
        /// <summary>
        /// Инициируем событие - смерть монстра
        /// </summary>
        static public void OnMonsterDied(Monster monster)
        {
            ChangeMoney(+monster.Cost);
            Score += monster.StartLife;

            UpdateInformationControl(ControlType.MoneyControl);
            UpdateInformationControl(ControlType.ScoreControl);

            if (MonsterDied != null)
                MonsterDied(new MonsterArg(monster));

            if (MonsterGone != null)
                MonsterGone(new MonsterArg(monster));
        }
        /// <summary>
        /// Инициируем событие - башня была выделена
        /// </summary>
        static public void OnTowerSelected(Tower tower)
        {
            if (TowerSelected != null)
                TowerSelected(new TowerArg(tower));
        }
        /// <summary>
        /// Инициируем событие - монстр выделен
        /// </summary>
        static public void OnMonsterSelected(Monster monster)
        {
            if (MonsterSelected != null)
                MonsterSelected(new MonsterArg(monster));
        }
        /// <summary>
        /// Инициируем событие - блокировка обнаружена
        /// </summary>
        static public void OnBlockDetected(BlockTestResult blockDetected)
        {
            if (BlockDetected != null)
                BlockDetected(new BlockDetectedArg(blockDetected));
        }

        public delegate void TowerHandler(TowerArg e);
        public delegate void MonsterHandler(MonsterArg e);

        static public event TowerHandler TowerSelected;
        
        static public event MonsterHandler MonsterSelected;
        static public event MonsterHandler MonsterPass;
        static public event MonsterHandler MonsterDied;
        static public event MonsterHandler MonsterGone;

        public delegate void BlockDetectedHandler(BlockDetectedArg e);
        static public event BlockDetectedHandler BlockDetected;

        public delegate void GameEventHandler();
        static public event GameEventHandler GameStarted;

        static public event GameLevel.LevelHandler GameOver;
        static public event GameLevel.LevelHandler CurrentLevelCompeted;
    }
}
