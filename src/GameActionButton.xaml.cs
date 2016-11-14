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
    /// Логика взаимодействия для GameActionButton.xaml
    /// </summary>
    public partial class GameActionButton : UserControl
    {
        public static DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(GameActionButton), new PropertyMetadata(new PropertyChangedCallback(OnImageChanged)));
        public static DependencyProperty ActionTextProperty = DependencyProperty.Register("ActionText", typeof(string), typeof(GameActionButton), new PropertyMetadata(new PropertyChangedCallback(OnActionTextChanged)));

        public static void OnImageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            GameActionButton control = (GameActionButton)sender;

            control.image.Source = (ImageSource)e.NewValue;
        }
        public static void OnActionTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            GameActionButton control = (GameActionButton)sender;

            control.actionLabel.Content = e.NewValue.ToString();
        }

        public ImageSource Image
        {
            get
            {
                return (ImageSource)GetValue(ImageProperty);
            }
            set
            {
                SetValue(ImageProperty, value);
            }
        }
        public string ActionText
        {
            get
            {
                return (string)GetValue(ActionTextProperty);
            }
            set
            {
                SetValue(ActionTextProperty, value);
            }
        }

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(GameActionButton));

        public event RoutedEventHandler Click
        {
            add
            {
                AddHandler(ClickEvent, value);
            }
            remove
            {
                RemoveHandler(ClickEvent, value);
            }
        }

        public GameActionButton()
        {
            InitializeComponent();
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs arg = new RoutedEventArgs(ClickEvent);
            RaiseEvent(arg);
        }
    }
}
