using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRTowerDefense
{
    /// <summary>
    /// Интерфейс взрываемых башен
    /// </summary>
    public interface IExplodeTower
    {
        /// <summary>
        /// Была ли взорвана башня
        /// </summary>
        bool Exploded { get; }
        /// <summary>
        /// Взорвать башню
        /// </summary>
        void Explode();
    }
}
