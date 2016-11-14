using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace NRTowerDefense
{

    public class TickArg
    {
        public double Interval { get; private set; }
        public TickArg(double interval)
        {
            Interval = interval;
        }
    }

    /// <summary>
    /// Игровой таймер. Обеспечивает оживление всех игровых объектов
    /// </summary>
    public class GameTimer
    {
        /// <summary>
        /// Интервал срабатывания таймера (в миллисекундах)
        /// </summary>
        public readonly int Interval;
        /// <summary>
        /// Счётчик числа тиков, с момента последнего сбрасывания
        /// </summary>
        public int TickCounter;
        /// <summary>
        /// Приостановить таймер
        /// </summary>
        public void Pause()
        {
            paused = true;
        }
        /// <summary>
        /// Возобновить работу таймера
        /// </summary>
        public void Unpause()
        {
            paused = false;
        }
        /// <summary>
        /// Сбросить число тиков
        /// </summary>
        public void Reset()
        {
            TickCounter = 0;
            lastUpdateTick = 0;
        }
        /// <summary>
        /// Конструктор таймера
        /// </summary>
        public GameTimer(int intervalInMilliSeconds)
        {
            Interval = intervalInMilliSeconds;
            TickCounter = 0;

            timer = new DispatcherTimer(DispatcherPriority.Send);
            timer.Tick += new EventHandler((s,e) => OnTick());
            timer.Interval = TimeSpan.FromMilliseconds(intervalInMilliSeconds);
            timer.Start();
        }        
        /// <summary>
        /// Возвращает сколько секунд от начала игры прошло
        /// </summary>
        public double SecondsElapsed()
        {
            return (Interval / 1000.0) * TickCounter;
        }
        /// <summary>
        /// Возвращает отформатированное время с начала игры mm:ss
        /// </summary>
        public string GetFormatedTime()
        {
            int timeElapsed = (int)SecondsElapsed();
            int sec = timeElapsed % 60;
            int min = (timeElapsed - sec) / 60;

            return string.Format("{0:00}:{1:00}", min, sec);
        }

        private int lastUpdateTick;
        private bool paused;
        private DispatcherTimer timer;
        private void OnTick()
        {
            if (!paused)
            {
                ++TickCounter;

                if (Tick != null)
                    Tick(this, new TickArg(Interval / 1000.0));

                if ((TickCounter - lastUpdateTick) * Interval > 500)
                {
                    Game.UpdateInformationControl(Game.ControlType.TimeControl);
                    lastUpdateTick = TickCounter;
                }
            }
        }
        
        public delegate void TickEventHandler(object sender, TickArg e);
        public event TickEventHandler Tick;
    }

}
