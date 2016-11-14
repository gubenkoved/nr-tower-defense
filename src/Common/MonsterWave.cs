using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace NRTowerDefense
{
    public class MonsterWave
    {
        private int currentMonster;
        /// <summary>
        /// Стратегия выхода монстров
        /// </summary>
        public WaveStrategy WaveStrategy { get; private set; }
        /// <summary>
        /// Монстры входящие в это нашествие
        /// </summary>
        public List<Monster> Monsters { get; private set; }
        /// <summary>
        /// Количество убитых или прошедших монстров из волны
        /// </summary>
        private int goneMonstersCount;
        /// <summary>
        /// Информация о волне
        /// </summary>
        public string Info;
        /// <summary>
        /// Связанное изображение
        /// </summary>
        public ImageSource LinkedImage;
        /// <summary>
        /// Конструктор
        /// </summary>
        public MonsterWave(WaveStrategy waveStraregy, string info = "", string imageName = null)
        {
            Info = info;
            WaveStrategy = waveStraregy;
            LinkedImage = (imageName != null) ? new BitmapImage(new Uri(imageName, UriKind.Relative)) : null;

            Monsters = new List<Monster>();
            goneMonstersCount = 0;
        }
        /// <summary>
        /// Начать выход монстров
        /// </summary>
        public void Go()
        {
            Game.Timer.Tick += new GameTimer.TickEventHandler(onGameTimerTick);
            Game.MonsterGone += new Game.MonsterHandler
                (
                    (a) =>
                        {
                            if (Monsters.Contains(a.Monster))
                            {
                                ++goneMonstersCount;

                                if (goneMonstersCount == Monsters.Count)
                                {
                                    if (AllMonstersDied != null)
                                        AllMonstersDied();
                                }
                            }
                        }
                );
            Game.GameOver += new GameLevel.LevelHandler
                (
                    (arg) => Game.Timer.Tick -= new GameTimer.TickEventHandler(onGameTimerTick)
                );
        }
        /// <summary>
        /// Реакция на тик
        /// </summary>
        private void onGameTimerTick(object sender, TickArg e)
        {
            if (currentMonster < Monsters.Count)
            {
                if (WaveStrategy.CanRunNextMonster())
                {
                    RunNextMonster();
                }
            }
        }
        /// <summary>
        /// Выпустить очередного монстра
        /// </summary>
        public void RunNextMonster()
        {
            if (currentMonster < Monsters.Count)
            {
                Game.Field.AddMonster(Monsters[currentMonster]);
                ++currentMonster;
            }
        }
        /// <summary>
        /// Добавить монстров в нашествие
        /// </summary>
        public void AddMonsters(Monster monster, int count)
        {
            for (int i = 1; i <= count; i++)
            {
                Monsters.Add(monster.Clone());
            }
        }

        public delegate void AllMonstersDiedHandler();
        public event AllMonstersDiedHandler AllMonstersDied;
    }
}
