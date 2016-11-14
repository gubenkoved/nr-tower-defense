using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRTowerDefense
{
    /// <summary>
    /// Основные игровые настройки
    /// </summary>
    public static class GameProperties
    {
        public static double RequireFPS = 100.0;

        public static int StartLife = 20;
        public static int StartMoney = 50;
        public static int StartScore = 0;

        public static int GameFieldCellSize = 20;
        public static int GameFieldIndention = 50;
        public static double GameFieldStartPositionDispersion = 0.24 * GameFieldCellSize;

        public static int ColorFieldAnimationZIndex         = int.MaxValue;
        public static int TowerAttackAreaZIndex             = int.MaxValue - 9;
        public static int BorderDecoratorZIndex             = int.MaxValue - 10;
        public static int MaxMonsterZIndex                  = int.MaxValue - 1000;
        public static int PrototypeZIndex                   = 1;
        public static int TowersLevelZIndex                 = -8;
        public static int TowersReloadingIndicatorZIndex    = -8;
        public static int TowersGunZIndex                   = -9;
        public static int TowersZIndex                      = -10;
        public static int DisappearanceImageGameAnimationZIndex = -11;
        public static int GameFieldStartEndPointZIndex      = int.MinValue + 2;
        public static int GameFieldStartToEndPathZIndex     = int.MinValue + 1;
        public static int GameFieldGridZIndex               = int.MinValue;

        public static double TowersReloadingIndicatorHeight = 3.0;
    }
}
