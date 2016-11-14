using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace NRTowerDefense
{
    public interface IHaveAttackArea
    {
        void DrawAttackArea(Canvas canvas);
        void UpdateAttackArea(Canvas canvas);
        void RemoveAttackArea(Canvas canvas);
    }
}
