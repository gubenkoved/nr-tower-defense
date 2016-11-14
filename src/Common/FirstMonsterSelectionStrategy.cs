using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRTowerDefense
{
    public class FirstMonsterSelectionStrategy : MonsterSelectionStrategy
    {
        
        public override Monster Select(List<Monster> monsters)
        {
            if (monsters.Count != 0)
            {
                foreach (Monster monster in monsters)
                {
                    if (Game.Field.CalculateSummaryDamage(monster, Game.Field.GetBulletsWithTarget(monster)) < monster.Life)
                    {
                        return monster;
                    }
                }
            }

            return null;
        }

        public override bool EachTickRecalculateTarget
        {
            get
            {
                return false;
            }
        }
    }
}
