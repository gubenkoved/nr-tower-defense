using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Helper;
using System.Collections.ObjectModel;

namespace NRTowerDefense
{

    public static class LevelDefinition
    {
        public static GameLevel Level1 { get; private set; }

        public static GameLevel Level2 { get; private set; }

        public static GameLevel Level3 { get; private set; }

        public static ObservableCollection<GameLevel> MainGameLevelList { get; private set; }

        static LevelDefinition()
        {
            MainGameLevelList = new ObservableCollection<GameLevel>();
        }

        public static void CreateLevels()
        {
            #region Levels
                #region Level1
                {
                    Level1 = new GameLevel("Newbie");

                    MonsterWave wave1 = new MonsterWave(new RegularWaveStrategy(1500 / Game.Timer.Interval));
                    wave1.AddMonsters(new SimpleMonster(new GameCell(3.0), 80, 5.0), 10);
                    wave1.LinkedImage = new BitmapImage(new Uri("/images/monster2.png", UriKind.Relative));
                    wave1.Info = "1. Обычные монстры";

                    MonsterWave wave2 = new MonsterWave(new GroupWaveStrategy());
                    wave2.AddMonsters(new SimpleMonster(new GameCell(3.0), 80, 5.0), 7);
                    wave2.LinkedImage = new BitmapImage(new Uri("/images/monster2.png", UriKind.Relative));
                    wave2.Info = "2. Группа монстров";

                    MonsterWave wave3 = new MonsterWave(new RegularWaveStrategy(2000 / Game.Timer.Interval));
                    wave3.AddMonsters(new ImmuneMonster(new GameCell(3.0), 150, 10, 3.0), 25);
                    wave3.LinkedImage = new BitmapImage(new Uri("/images/monster3.png", UriKind.Relative));
                    wave3.Info = "3. Эти монстры устойчивы к заморозке";

                    MonsterWave wave4 = new MonsterWave(new RegularWaveStrategy(5000 / Game.Timer.Interval));
                    wave4.AddMonsters(new DarkMonster(new GameCell(2.0), 1000.0, 75.0, 10.0), 1);
                    wave4.LinkedImage = new BitmapImage(new Uri("/images/monster4.png", UriKind.Relative));
                    wave4.Info = "4. Это - босс, у него много жизней";

                    MonsterWave wave5 = new MonsterWave(new RegularWaveStrategy(1000 / Game.Timer.Interval));
                    wave5.AddMonsters(new SimpleMonster(new GameCell(3.0), 200, 15), 15);
                    wave5.LinkedImage = new BitmapImage(new Uri("/images/monster2.png", UriKind.Relative));
                    wave5.Info = "5. Усиленные монстры";

                    MonsterWave wave6 = new MonsterWave(new RegularWaveStrategy(1000 / Game.Timer.Interval));
                    wave6.AddMonsters(new ImmuneMonster(new GameCell(3.0), 300, 15, 4.0), 10);
                    wave6.LinkedImage = new BitmapImage(new Uri("/images/monster3.png", UriKind.Relative));
                    wave6.Info = "6. Устойчивые усиленные монстры";

                    MonsterWave wave7 = new MonsterWave(new RegularWaveStrategy(800 / Game.Timer.Interval));
                    wave7.AddMonsters(new SimpleMonster(new GameCell(3.0), 350, 15), 30);
                    wave7.LinkedImage = new BitmapImage(new Uri("/images/monster2.png", UriKind.Relative));
                    wave7.Info = "7. Большое нашествие";

                    MonsterWave wave8 = new MonsterWave(new RegularWaveStrategy(5000 / Game.Timer.Interval));
                    wave8.AddMonsters(new DarkMonster(new GameCell(2.0), 3000.0, 150.0, 30.0), 2);
                    wave8.LinkedImage = new BitmapImage(new Uri("/images/monster4.png", UriKind.Relative));
                    wave8.Info = "8. Заключительная босс-волна";

                    Level1.AddWave(wave1, 20);
                    Level1.AddWave(wave2, 20);
                    Level1.AddWave(wave3, 50);
                    Level1.AddWave(wave4, 40);
                    Level1.AddWave(wave5, 20);
                    Level1.AddWave(wave6, 20);
                    Level1.AddWave(wave7, 30);
                    Level1.AddWave(wave8, 30);
                }
                #endregion
                #region Level2
                {
                    Level2 = new GameLevel("Professional");

                    MonsterWave wave1 = new MonsterWave(new RegularWaveStrategy(1000 / Game.Timer.Interval));
                    wave1.AddMonsters(new SimpleMonster(new GameCell(3.0), 80, 8.0), 15);
                    wave1.LinkedImage = new BitmapImage(new Uri("/images/monster2.png", UriKind.Relative));
                    wave1.Info = "1. Запасайтесь деньгами";

                    MonsterWave wave2 = new MonsterWave(new RegularWaveStrategy(1800 / Game.Timer.Interval));
                    wave2.AddMonsters(new ImmuneMonster(new GameCell(3.0), 120, 10.0), 10);
                    wave2.LinkedImage = new BitmapImage(new Uri("/images/monster3.png", UriKind.Relative));
                    wave2.Info = "2. Готовтесь =)";

                    MonsterWave wave3 = new MonsterWave(new GroupWaveStrategy());
                    wave3.AddMonsters(new SimpleMonster(new GameCell(3.0), 100, 13.0, 2.0), 8);
                    wave3.LinkedImage = new BitmapImage(new Uri("/images/monster2.png", UriKind.Relative));
                    wave3.Info = "3. Are you ready?";

                    MonsterWave wave4 = new MonsterWave(new RegularWaveStrategy(3500 / Game.Timer.Interval));
                    wave4.AddMonsters(new DarkMonster(new GameCell(3.0), 666, 50.0, 5.0), 5);
                    wave4.LinkedImage = new BitmapImage(new Uri("/images/monster4.png", UriKind.Relative));
                    wave4.Info = "4. Волна боссов";

                    MonsterWave wave5 = new MonsterWave(new RegularWaveStrategy(1000 / Game.Timer.Interval));
                    wave5.AddMonsters(new ImmuneMonster(new GameCell(3.0), 150, 15.0, 4.0), 10);
                    wave5.LinkedImage = new BitmapImage(new Uri("/images/monster3.png", UriKind.Relative));
                    wave5.Info = "5-я волна";

                    MonsterWave wave6 = new MonsterWave(new RegularWaveStrategy(1000 / Game.Timer.Interval));
                    wave6.AddMonsters(new ImmuneMonster(new GameCell(3.0), 200, 17.0, 7.0), 10);
                    wave6.LinkedImage = new BitmapImage(new Uri("/images/monster3.png", UriKind.Relative));
                    wave6.Info = "6-я волна";

                    MonsterWave wave7 = new MonsterWave(new RegularWaveStrategy(1000 / Game.Timer.Interval));
                    wave7.AddMonsters(new ImmuneMonster(new GameCell(3.0), 250, 18.0, 10.0), 10);
                    wave7.LinkedImage = new BitmapImage(new Uri("/images/monster3.png", UriKind.Relative));
                    wave7.Info = "7-я волна";

                    MonsterWave wave8 = new MonsterWave(new RegularWaveStrategy(1000 / Game.Timer.Interval));
                    wave8.AddMonsters(new ImmuneMonster(new GameCell(3.0), 300, 20.0, 9.0), 10);
                    wave8.LinkedImage = new BitmapImage(new Uri("/images/monster3.png", UriKind.Relative));
                    wave8.Info = "8-я волна";

                    MonsterWave wave9 = new MonsterWave(new RegularWaveStrategy(250 / Game.Timer.Interval));
                    wave9.AddMonsters(new SimpleMonster(new GameCell(3.0), 300, 20.0, 9.0), 60);
                    wave9.LinkedImage = new BitmapImage(new Uri("/images/monster2.png", UriKind.Relative));
                    wave9.Info = "9. БОЛЬШОЕ нашествие";

                    MonsterWave wave10 = new MonsterWave(new GroupWaveStrategy());
                    wave10.AddMonsters(new ImmuneMonster(new GameCell(3.0), 400, 15.0, 15.0), 30);
                    wave10.LinkedImage = new BitmapImage(new Uri("/images/monster3.png", UriKind.Relative));
                    wave10.Info = "10. БОЛЬШАЯ группа монстров";

                    MonsterWave wave11 = new MonsterWave(new RegularWaveStrategy(5000 / Game.Timer.Interval));
                    wave11.AddMonsters(new DarkMonster(new GameCell(3.0), 10000, 100.0, 20.0), 5);
                    wave11.LinkedImage = new BitmapImage(new Uri("/images/monster4.png", UriKind.Relative));
                    wave11.Info = "11. Несколько боссов";

                    Level2.AddWave(wave1, 18);
                    Level2.AddWave(wave2, 18);
                    Level2.AddWave(wave3, 18);
                    Level2.AddWave(wave4, 18);
                    Level2.AddWave(wave5, 12);
                    Level2.AddWave(wave6, 12);
                    Level2.AddWave(wave7, 12);
                    Level2.AddWave(wave8, 12);
                    Level2.AddWave(wave9, 40);
                    Level2.AddWave(wave10, 40);
                    Level2.AddWave(wave11, 60);
                } 
                #endregion
                #region Level3
                {
                    Level3 = new GameLevel("Godlike");

                    MonsterWave wave1 = new MonsterWave(new RegularWaveStrategy(1200 / Game.Timer.Interval));
                    wave1.AddMonsters(new SimpleMonster(new GameCell(3.5), 80, 10.0), 10);
                    wave1.LinkedImage = new BitmapImage(new Uri("/images/monster2.png", UriKind.Relative));
                    wave1.Info = "1-я волна";

                    MonsterWave wave2 = new MonsterWave(new RegularWaveStrategy(1800 / Game.Timer.Interval));
                    wave2.AddMonsters(new ImmuneMonster(new GameCell(4.0), 140, 12.0, 3.0), 10);
                    wave2.LinkedImage = new BitmapImage(new Uri("/images/monster3.png", UriKind.Relative));
                    wave2.Info = "2-я волна";

                    MonsterWave wave3 = new MonsterWave(new RegularWaveStrategy(150 / Game.Timer.Interval));
                    wave3.AddMonsters(new SimpleMonster(new GameCell(3.5), 20, 15.0, 0.0), 30 );
                    wave3.LinkedImage = new BitmapImage(new Uri("/images/monster2.png", UriKind.Relative));
                    wave3.Info = "3. Нужна \"зелень\"? Держите!";

                    MonsterWave wave4 = new MonsterWave(new RegularWaveStrategy(1500 / Game.Timer.Interval));
                    wave4.AddMonsters(new ImmuneMonster(new GameCell(3.0), 150, 20.0, 10.0), 10);
                    wave4.LinkedImage = new BitmapImage(new Uri("/images/monster3.png", UriKind.Relative));
                    wave4.Info = "4-я волна";

                    MonsterWave wave5 = new MonsterWave(new RegularWaveStrategy(1500 / Game.Timer.Interval));
                    wave5.AddMonsters(new ImmuneMonster(new GameCell(3.0), 200, 25.0, 15.0), 10);
                    wave5.LinkedImage = new BitmapImage(new Uri("/images/monster3.png", UriKind.Relative));
                    wave5.Info = "5-я волна";

                    MonsterWave wave6 = new MonsterWave(new RegularWaveStrategy(1500 / Game.Timer.Interval));
                    wave6.AddMonsters(new ImmuneMonster(new GameCell(3.0), 250, 30.0, 20.0), 10);
                    wave6.LinkedImage = new BitmapImage(new Uri("/images/monster3.png", UriKind.Relative));
                    wave6.Info = "6-я волна";

                    MonsterWave wave7 = new MonsterWave(new RegularWaveStrategy(1500 / Game.Timer.Interval));
                    wave7.AddMonsters(new ImmuneMonster(new GameCell(3.0), 300, 35.0, 25.0), 10);
                    wave7.LinkedImage = new BitmapImage(new Uri("/images/monster3.png", UriKind.Relative));
                    wave7.Info = "7-я волна";

                    MonsterWave wave8 = new MonsterWave(new RegularWaveStrategy(1500 / Game.Timer.Interval));
                    wave8.AddMonsters(new ImmuneMonster(new GameCell(3.0), 350, 40.0, 30.0), 10);
                    wave8.LinkedImage = new BitmapImage(new Uri("/images/monster3.png", UriKind.Relative));
                    wave8.Info = "8-я волна";

                    MonsterWave wave9 = new MonsterWave(new RegularWaveStrategy(4000 / Game.Timer.Interval));
                    wave9.AddMonsters(new DarkMonster(new GameCell(2.5), 5000, 500.0, 40.0), 3);
                    wave9.LinkedImage = new BitmapImage(new Uri("/images/monster4.png", UriKind.Relative));
                    wave9.Info = "9-я волна";

                    MonsterWave wave10 = new MonsterWave(new RegularWaveStrategy(300 / Game.Timer.Interval));
                    wave10.AddMonsters(new SimpleMonster(new GameCell(3.0), 300, 15, 10.0), 100);
                    wave10.LinkedImage = new BitmapImage(new Uri("/images/monster2.png", UriKind.Relative));
                    wave10.Info = "10. БОЛЬШОЕ нашествие";

                    MonsterWave wave11 = new MonsterWave(new GroupWaveStrategy());
                    wave11.AddMonsters(new ImmuneMonster(new GameCell(3.0), 360, 15, 20.0), 100);
                    wave11.LinkedImage = new BitmapImage(new Uri("/images/monster3.png", UriKind.Relative));
                    wave11.Info = "11. ПОСЛЕДНЕЕ испытание";

                    Level3.AddWave(wave1, 20);
                    Level3.AddWave(wave2, 20);
                    Level3.AddWave(wave3, 20);
                    Level3.AddWave(wave4, 20);
                    Level3.AddWave(wave5, 20);
                    Level3.AddWave(wave6, 20);
                    Level3.AddWave(wave7, 20);
                    Level3.AddWave(wave8, 20);
                    Level3.AddWave(wave9, 50);
                    Level3.AddWave(wave10, 60);
                    Level3.AddWave(wave11, 30);
                }
                #endregion
            #endregion
            #region MainGameLevelList
                MainGameLevelList.Clear();
                MainGameLevelList.Add(Level1);
                MainGameLevelList.Add(Level2);
                MainGameLevelList.Add(Level3);
	        #endregion
        }
    }
}
