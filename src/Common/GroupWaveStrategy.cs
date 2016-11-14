using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRTowerDefense
{
    public class GroupWaveStrategy : WaveStrategy
    {
        private int lastMonsterGoTick;

        public override bool CanRunNextMonster()
        {
            if (Game.Timer.TickCounter - lastMonsterGoTick > 3 )
            {
                lastMonsterGoTick = Game.Timer.TickCounter;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string GetName()
        {
            return "Группа";
        }
    }
}
