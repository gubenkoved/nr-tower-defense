using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRTowerDefense
{
    /// <summary>
    /// Интерфейс не статичных объектов
    /// </summary>
    public interface ITickable
    {
        void Tick(double interval);
    }
}
