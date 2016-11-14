using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRTowerDefense
{
    public interface IUnfreezebleMonster
    {
        void TryFreeze(Bullet bullet);
    }
}
