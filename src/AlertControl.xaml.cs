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
    /// Логика взаимодействия для BlockDetectAlertControl.xaml
    /// </summary>
    public partial class AlertControl : UserControl
    {
        System.Windows.Threading.DispatcherTimer timer;
        private double timerInterval = 0.500;
        public double leftTime;

        public AlertControl()
        {
            InitializeComponent();
        }

        public void ShowPopup(UIElement target, string message, double time)
        {
            MessageText.Text = message;
            ContainerPopup.PlacementTarget = target;
            ContainerPopup.IsOpen = true;
            leftTime = time;

            timer = new System.Windows.Threading.DispatcherTimer(System.Windows.Threading.DispatcherPriority.Send);
            timer.Interval = TimeSpan.FromMilliseconds(timerInterval * 1000);

            timer.Tick += new EventHandler(
                    (s, e) => 
                        {
                            leftTime -= timerInterval;
                            if (leftTime <= 0)
                            {
                                timer.Stop();
                                timer = null;
                                ContainerPopup.IsOpen = false;
                            }
                        }
                    );

            timer.Start();
        }
    }
}
