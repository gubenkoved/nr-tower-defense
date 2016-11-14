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
using System.Windows.Threading;

namespace NRTowerDefense
{
    /// <summary>
    /// Логика взаимодействия для GameLevelPresenterControl.xaml
    /// </summary>
    public partial class GameLevelPresenterControl : UserControl
    {
        private GameLevel gameLevel;
        private DispatcherTimer updateTimer;

        public void SetLevel(GameLevel level)
        {
            gameLevel = level;

            updateControls();

            WaveStackPanel.Margin = new Thickness(0.0);
            StartLevelBorder.Visibility = System.Windows.Visibility.Visible;
            WaveStackPanel.Visibility = System.Windows.Visibility.Visible;
        }

        private void updateControls()
        {
            WaveStackPanel.Children.Clear();

            for (int i = 0; i < gameLevel.Waves.Count; ++i)
            {
                MonsterWave wave = gameLevel.Waves[ i ];

                GameWavePresenterControl wavePresenter = new GameWavePresenterControl();
                WaveStackPanel.Children.Add(wavePresenter);
                wavePresenter.SetWave(wave);
            }
        }

        private void updateProgress()
        {
            if (gameLevel != null)
            {
                if (gameLevel.Status == GameLevel.LevelStatus.InProgress)
                {
                    int wave = gameLevel.ActiveWave;
                    GameWavePresenterControl wavePresenter = WaveStackPanel.Children[wave] as GameWavePresenterControl;

                    double sumWidthBefore = 0.0;
                    for (int i = 0; i < wave; i++)
                    {
                        sumWidthBefore += (WaveStackPanel.Children[i] as GameWavePresenterControl).ActualWidth;
                    }

                    WaveStackPanel.Margin = new Thickness(-1 * (sumWidthBefore + gameLevel.GetWaveProgress() * wavePresenter.ActualWidth), 0, 0, 0);
                }
            }
        }

        public GameLevelPresenterControl()
        {
            InitializeComponent();

            updateTimer = new DispatcherTimer(System.Windows.Threading.DispatcherPriority.Send);
            updateTimer.Tick += new EventHandler((s, e) => updateProgress());
            updateTimer.Interval = TimeSpan.FromMilliseconds(100);
            updateTimer.Start();
        }

        private void NextWaveBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (gameLevel.Status == GameLevel.LevelStatus.InProgress)
                gameLevel.GoNextWave();
        }

        private void StartLevelBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (gameLevel != null)
            {
                if (gameLevel.Status == GameLevel.LevelStatus.NotStarted)
                {
                    gameLevel.Start();
                    StartLevelBorder.Visibility = System.Windows.Visibility.Hidden;
                }
            }
        }
    }
}
