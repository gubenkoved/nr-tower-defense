using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace NRTowerDefense
{
    /// <summary>
    /// Интерфейс отрисовки на канвас
    /// </summary>
    public interface IDrawable
    {
        void Draw(Canvas canvas);
        void ClearFromCanvas(Canvas canvas);
    }
}
