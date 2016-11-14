using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRTowerDefense
{
    public class LevelArg
    {
        public GameLevel Level { get; private set; }
        public LevelArg(GameLevel level)
        {
            Level = level;
        }
    }

    /// <summary>
    /// Игровой уровень
    /// </summary>
    public class GameLevel
    {
        /// <summary>
        /// Статус игрового уровня
        /// </summary>
        public enum LevelStatus
        {
            Completed, InProgress, NotStarted
        }
        /// <summary>
        /// Статус игрового уровня
        /// </summary>
        public LevelStatus Status { get; private set; }
        /// <summary>
        /// Название уровня
        /// </summary>
        public string Name { private set; get; }
        /// <summary>
        /// Переопределяем object.ToString()
        /// </summary>
        public override string ToString()
        {
            return Name;
        }
        /// <summary>
        /// Волны монстров в уровне
        /// </summary>
        public List<MonsterWave> Waves { get; private set; }
        /// <summary>
        /// Длительность соответствующей волны
        /// </summary>
        public List<double> WaveTime { get; private set; }
        /// <summary>
        /// Текущая волна
        /// </summary>
        public int ActiveWave { get; private set; }
        /// <summary>
        /// Время в секундах от начала волны
        /// </summary>
        private double activeWaveStartTime;
        /// <summary>
        /// Число завершённых волн
        /// </summary>
        private int completedWaves;
        /// <summary>
        /// Возвращает степень завершённости волны
        /// </summary>
        public double GetWaveProgress()
        {
            if (Status == LevelStatus.InProgress)
            {
                if (ActiveWave < Waves.Count)
                    return (Game.Timer.SecondsElapsed() - activeWaveStartTime) / WaveTime[ActiveWave];
                else
                    return double.NaN;
            }
            else
            {
                return 0;
            }
            
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        public GameLevel(string name)
        {
            Waves = new List<MonsterWave>();
            WaveTime = new List<double>();
            
            Name = name;
            Status = LevelStatus.NotStarted;
            ActiveWave = -1;
        }
        /// <summary>
        /// Добавить волну в уровень
        /// </summary>
        public void AddWave(MonsterWave wave, double period = 10.0)
        {
            Waves.Add(wave);
            WaveTime.Add(period);
        }
        /// <summary>
        /// Начать уровень
        /// </summary>
        public void Start()
        {
            if (Waves.Count != 0)
            {
                Status = LevelStatus.InProgress;
                
                Game.Timer.Tick += new GameTimer.TickEventHandler((s,e) => onGameTimerTick());

                GoNextWave();
            }
        }
        /// <summary>
        /// Высылает следующую волну
        /// </summary>
        public void GoNextWave()
        {
            if (ActiveWave != Waves.Count - 1)
            {
                ++ActiveWave;

                activeWaveStartTime = Game.Timer.SecondsElapsed();

                Waves[ActiveWave].Go();

                Waves[ActiveWave].AllMonstersDied += new MonsterWave.AllMonstersDiedHandler
                    (() =>
                        {
                            ++completedWaves;

                            if (completedWaves == Waves.Count && Completed != null)
                                Completed(new LevelArg(this));
                        });
            }
        }
        /// <summary>
        /// Реакция на тик
        /// </summary>
        private void onGameTimerTick()
        {
            if (Status == LevelStatus.InProgress)
            {
                if (ActiveWave < Waves.Count)
                {
                    if (Game.Timer.SecondsElapsed() - activeWaveStartTime >= WaveTime[ActiveWave])
                    {
                        GoNextWave();
                    }
                }
            }
        }

        public delegate void LevelHandler(LevelArg arg);
        public event LevelHandler Completed;
    }
}
