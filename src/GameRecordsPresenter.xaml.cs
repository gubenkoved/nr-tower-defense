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
using System.Windows.Shapes;

namespace NRTowerDefense
{
    /// <summary>
    /// Логика взаимодействия для GameRecordsPresenter.xaml
    /// </summary>
    public partial class GameRecordsPresenter : Window
    {
        private GameRecords gameRecords;

        public GameRecordsPresenter()
        {
            InitializeComponent();

            gameRecords = new GameRecords(GameRecords.DEFAULT_FILE_NAME);
        }

        private void CloseLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            presentRecords();
        }

        private void presentRecords()
        {
            IEnumerable<string> levels = (from record in gameRecords.Records
                                          select record.LevelName).Distinct();

            foreach (var level in levels)
            {
                TabItem levelTab = new TabItem();
                levelTab.Header = string.Format("Уровень \"{0}\"",level);
                levelTab.Background = new LinearGradientBrush(Color.FromRgb(0x95,0xC2,0xF5), Color.FromRgb(0x0C,0x67,0xCF), 90);

                ListBox recordsList = new ListBox();
                recordsList.Style = (Style)this.FindResource("ListBoxStyle");
                recordsList.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
                recordsList.ItemsSource = from record in gameRecords.Records
                                          where record.LevelName == level
                                          orderby record.Score descending
                                          select record;

                recordsList.ItemTemplate = (DataTemplate)this.FindResource("RecordTemplate");
                levelTab.Content = recordsList;

                LevelsTabControl.Items.Add(levelTab);
            }
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
