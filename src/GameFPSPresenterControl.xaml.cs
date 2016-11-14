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
    /// Логика взаимодействия для GameFPSPresenterControl.xaml
    /// </summary>
    public partial class GameFPSPresenterControl : UserControl
    {
        private const int updateEvery = 30; // tick
        private int lastEnvironmentTickCount;
        private int tickCounter;

        public GameFPSPresenterControl()
        {
            InitializeComponent();

            lastEnvironmentTickCount = Environment.TickCount;
        }

        /// <summary>
        /// Для подсчёта FPS вызывать на каждом тике
        /// </summary>
        public void Tick()
        {
            ++tickCounter;

            if (tickCounter % updateEvery == 0)
            {
                double fps = updateEvery / (0.001 * (Environment.TickCount - lastEnvironmentTickCount));
                lastEnvironmentTickCount = Environment.TickCount;

                FPSText.Text = fps.ToString("0.0");
            }
        }
    }
}
