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
    public class GameCellToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((GameCell)value).Amount + "×";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Логика взаимодействия для GameMonsterInfoControl.xaml
    /// </summary>
    public partial class GameMonsterInfoControl : UserControl
    {
        public Monster Monster
        {
            get { return (Monster)GetValue(MonsterProperty); }
            set { SetValue(MonsterProperty, value); }
        }

        public static readonly DependencyProperty MonsterProperty = DependencyProperty.Register("MonsterProperty", typeof(Monster), typeof(GameMonsterInfoControl), new PropertyMetadata(new PropertyChangedCallback(OnMonsterChanged)));

        public static void OnMonsterChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            GameMonsterInfoControl control = (GameMonsterInfoControl)sender;

            if (e.NewValue != null)
            {
                (e.NewValue as Monster).PropertyChanged += (s, arg) => control.monsterParametersChanged();
                control.presentWithMonsterInfo(e.NewValue as Monster);
                control.updateMonsterImage();
                control.updateMonsterInfo();
            }
            else
            {
                control.presentAsEmpty();
            }
        }

        private void monsterParametersChanged()
        {
            if (Monster.Killed)
            {
                updateMessage("Выбранный монстр был убит или покинул поле...");
                showMessage();
            }
        }

        private void updateMessage( string message )
        {
            MessageText.Text = message;
        }

        private void presentAsEmpty()
        {
            DataContext = null;

            updateMessage("Выберите монстра для просмотра его параметров...");

            showMessage();
        }

        private void showMessage()
        {
            MessageBorder.Visibility = System.Windows.Visibility.Visible;
            InfoGrid.Visibility = System.Windows.Visibility.Hidden;
        }

        private void hideMessage()
        {
            MessageBorder.Visibility = System.Windows.Visibility.Hidden;
            InfoGrid.Visibility = System.Windows.Visibility.Visible;
        }

        private void presentWithMonsterInfo( Monster monster )
        {
            DataContext = monster;

            if (monster is IUnfreezebleMonster)
                FreezePosibilityLabel.Content = "иммунитет";
            else
                FreezePosibilityLabel.Content = "замораживаемый";

            hideMessage();
        }

        public GameMonsterInfoControl()
        {
            InitializeComponent();
            
            presentAsEmpty();
        }

        private void updateMonsterImage()
        {
            if (Monster is SimpleMonster)
            {
                MonsterImage.Source = new BitmapImage(new Uri("/images/monster2.png", UriKind.Relative));
            } else if (Monster is ImmuneMonster)
            {
                MonsterImage.Source = new BitmapImage(new Uri("/images/monster3.png", UriKind.Relative));
            } else if (Monster is DarkMonster)
            {
                MonsterImage.Source = new BitmapImage(new Uri("/images/monster4.png", UriKind.Relative));
            }
        }

        private void updateMonsterInfo()
        {
            MonsterMainLabel.Content = Monster.GetType().GetField("Description").GetValue(null);
        }
    }
}
