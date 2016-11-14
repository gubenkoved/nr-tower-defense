using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRTowerDefense
{
    public abstract class TowerDecorator : GameObjectDecorator
    {
        /// <summary>
        /// Замещаем kernel родителя
        /// </summary>
        public new Tower Kernel
        {
            get
            {
                if (component is TowerDecorator)
                    return (component as TowerDecorator).Kernel;
                else
                    return component as Tower;
            }
        }

        public TowerDecorator(Tower component)
            : base(component)
        {
        }

        public TowerDecorator(TowerDecorator component)
            : base(component)
        {
        }
    }
}
