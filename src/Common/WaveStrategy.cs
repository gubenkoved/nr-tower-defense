using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRTowerDefense
{
    public abstract class WaveStrategy
    {
        public abstract bool CanRunNextMonster();
        public abstract string GetName();
    }
}
