using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRTowerDefense
{
    public interface IUpgradeableTower
    {
        int Level { set; get; }
        int MaxLevel { get; }
        double[] CostUpgradeScale { get; }
        double[] BulletDamageUpgradeScale { get; }
        double[] ReloadingSpeedUpgradeScale { get; }
        GameCell[] BulletSpeedUpgradeScale { get; }
        GameCell[] AttackRadiusUpgradeScale { get; }
        void Upgrade();
    }

    public interface IUpgradeableTowerWithFreezeBullet : IUpgradeableTower
    {
        double[] FreezeTimeUpgradeScale { get; }
        double[] FreezeFactorUpgradeScale { get; }

        void FreezeUpgrade();
    }
}
