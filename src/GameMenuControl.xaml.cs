using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Globalization;

namespace NRTowerDefense
{
    public class SelectedItemToEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Логика взаимодействия для GameOverControl.xaml
    /// </summary>
    public partial class GameMenuControl : UserControl
    {
        private GameLevelPresenterControl levelPresenter;

        public GameMenuControl( )
        {
            InitializeComponent();
        }

        public void SetLevelSource(ObservableCollection<GameLevel> levelCollection)
        {
            Levels.ItemsSource = levelCollection;
        }

        public void SetLevelPresenter( GameLevelPresenterControl levelPresenter)
        {
            this.levelPresenter = levelPresenter;
        }

        public void SetHeader(object obj)
        {
            Header.Content = obj;
        }

        public void SetMessage(object obj)
        {
            Message.Content = obj;
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            var level = Levels.SelectedItem as GameLevel;
            LevelDefinition.CreateLevels();
            
            Game.NewGame(level);
            levelPresenter.SetLevel(level);

            Levels.SelectedItem = null;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void RecordsButton_Click(object sender, RoutedEventArgs e)
        {
            GameRecordsPresenter recordsPresenter = new GameRecordsPresenter();
            recordsPresenter.ShowDialog();
        }

    }
}
