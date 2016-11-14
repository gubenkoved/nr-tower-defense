using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRTowerDefense
{
    public interface IFreezeBullet
    {
        double FreezeFactor { get; set; }
        double FreezeTime { get; set; }
    }
}
