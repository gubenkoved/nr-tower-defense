using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRTowerDefense
{
    public abstract class MonsterSelectionStrategy
    {
        public abstract Monster Select(List<Monster> monsters);
        public abstract bool EachTickRecalculateTarget { get; }
    }
}
