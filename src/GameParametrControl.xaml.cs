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
    /// Логика взаимодействия для GameParametrControl.xaml
    /// </summary>
    public partial class GameParametrControl : UserControl
    {
        public static DependencyProperty ImageSourceProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(GameParametrControl), new PropertyMetadata(new PropertyChangedCallback(OnImageChanged)));
        public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(GameParametrControl), new PropertyMetadata(new PropertyChangedCallback(OnDataChanged)));
        public static DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(GameParametrControl), new PropertyMetadata(new PropertyChangedCallback(OnDataChanged)));
        public static DependencyProperty IncrementProperty = DependencyProperty.Register("Increment", typeof(double), typeof(GameParametrControl), new PropertyMetadata(new PropertyChangedCallback(OnDataChanged)));
        public static DependencyProperty InGameCellProperty = DependencyProperty.Register("InGameCell", typeof(bool), typeof(GameParametrControl), new PropertyMetadata(new PropertyChangedCallback(OnDataChanged)));

        public static void OnImageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            GameParametrControl control = (GameParametrControl)sender;

            control.image.Source = (ImageSource) e.NewValue;
        }
        public static void OnDataChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            GameParametrControl control = (GameParametrControl)sender;

            if (e.Property == TextProperty)
                control.text.Content = (string)e.NewValue;
            else if (e.Property == ValueProperty)
            {
                double d = (double)e.NewValue;

                if (double.IsNaN(d))
                {
                    control.value.Content = "-";
                }
                else if (double.IsPositiveInfinity(d))
                {
                    control.value.Content = "беск.";
                }
                else if (double.IsNegativeInfinity(d))
                {
                    control.value.Content = "-беск.";
                }
                else
                {
                    control.value.Content = d.ToString();

                    if (control.InGameCell)
                        control.value.Content += "×";
                }
            }
            else if (e.Property == IncrementProperty)
            {
                double d = (double)e.NewValue;
                control.increment.Content = d.ToString("+0.##;-0.##;←");
                control.increment.Foreground = new SolidColorBrush(Colors.Green);

                if (double.IsNaN(d))
                {
                    control.increment.Content = "-";
                }
                else if (double.IsPositiveInfinity(d))
                {
                    control.increment.Content = "∞";
                }
                else if (double.IsNegativeInfinity(d))
                {
                    control.increment.Foreground = new SolidColorBrush(Colors.Red);
                    control.increment.Content = "-∞";
                }
                else
                {
                    if (d != 0)
                    {
                        if (d < 0)
                            control.increment.Foreground = new SolidColorBrush(Colors.Red);

                        if (control.InGameCell)
                            control.increment.Content += "×";
                    }
                }
            }
        }

        public ImageSource Image
        {
            get
            {
                return (ImageSource)GetValue(ImageSourceProperty);
            }
            set
            {
                SetValue(ImageSourceProperty, value);
            }
        }
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }
        public double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }
        public double Increment
        {
            get
            {
                return (double)GetValue(IncrementProperty);
            }
            set
            {
                SetValue(IncrementProperty, value);
            }
        }
        public bool InGameCell
        {
            get
            {
                return (bool)GetValue(InGameCellProperty);
            }
            set
            {
                SetValue(InGameCellProperty, value);
            }
        }

        public void ResetData()
        {
            Value = double.NaN;
            Increment = double.NaN;
            increment.Foreground = new SolidColorBrush(Colors.Green);
        }

        public GameParametrControl()
        {
            InitializeComponent();
        }
    }
}
