using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NRTowerDefense
{
    /// <summary>
    /// Логика взаимодействия для GameWavePresenterControl.xaml
    /// </summary>
    public partial class GameWavePresenterControl : UserControl
    {
        private MonsterWave monsterWave;
        public void SetWave(MonsterWave wave)
        {
            monsterWave = wave;
            updateControls();
        }
        private void updateControls()
        {
            CounterText.Text = monsterWave.Monsters.Count.ToString("x#");
            WaveStrategyText.Text = monsterWave.WaveStrategy.GetName();
            WaveImage.Source = monsterWave.LinkedImage;

            InfoText.Text = monsterWave.Info;

            if (monsterWave.Info == "")
                InfoText.Visibility = System.Windows.Visibility.Collapsed;
        }

        public GameWavePresenterControl()
        {
            InitializeComponent();
        }
    }
}
