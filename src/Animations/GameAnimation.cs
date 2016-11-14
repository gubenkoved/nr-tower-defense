using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace NRTowerDefense
{
    /// <summary>
    /// Абстрактный класс - игровая анимация
    /// </summary>
    public abstract class GameAnimation
    {
        /// <summary>
        /// Отображаемые UIElement'ы
        /// </summary>
        protected List<UIElement> visuals = new List<UIElement>();
        /// <summary>
        /// Флаг, закончилась ли анимация
        /// </summary>
        public bool Finished;

        public abstract void StartAnimation(Canvas canvas);
        public abstract void ClearFromCanvas(Canvas canvas);
    }
}
