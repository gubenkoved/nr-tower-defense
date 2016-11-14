using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRTowerDefense
{
    class NotFreezedMonsterSelectionStrategy : MonsterSelectionStrategy
    {
        public override Monster Select(List<Monster> monsters)
        {
            Monster result = null;
            double minFreeze = double.MaxValue;

            foreach (Monster monster in monsters)
            {
                if (!(monster is IUnfreezebleMonster))
                {
                    if (monster.FreezeFactor * monster.FreezeTime < minFreeze)
                    {
                        result = monster;
                        minFreeze = monster.FreezeFactor * monster.FreezeTime;
                    }
                }
            }

            return result;
        }

        public override bool EachTickRecalculateTarget
        {
            get
            {
                return true;
            }
        }
    }
}
