using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRTowerDefense
{
    public abstract class MonsterDecorator : GameObjectDecorator
    {
        /// <summary>
        /// Замещаем kernel родителя
        /// </summary>
        public new Monster Kernel
        {
            get
            {
                if (component is MonsterDecorator)
                    return (component as MonsterDecorator).Kernel;
                else
                    return component as Monster;
            }
        }

        public MonsterDecorator(Monster component)
            : base( component )
        {
        }

        public MonsterDecorator(MonsterDecorator component)
            : base(component)
        {
        }
    }
}
