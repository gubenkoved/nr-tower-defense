using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRTowerDefense
{
    public class RegularWaveStrategy : WaveStrategy
    {
        private int lastMonsterGoTick;

        private int monsterTickDistance;

        public RegularWaveStrategy(int monsterTickDistance = 10)
        {
            this.monsterTickDistance = monsterTickDistance;
        }

        public override bool CanRunNextMonster()
        {
            if (Game.Timer.TickCounter - lastMonsterGoTick > monsterTickDistance)
            {
                lastMonsterGoTick = Game.Timer.TickCounter;
                return true;
            } else
            {
                return false;
            }
        }

        public override string GetName()
        {
            return "Колонна";
        }
    }
}
