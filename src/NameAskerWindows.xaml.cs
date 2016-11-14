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
    /// Логика взаимодействия для NameAskerWindows.xaml
    /// </summary>
    public partial class NameAskerWindows : Window
    {
        public string PlayerName { get; private set; }

        public NameAskerWindows()
        {
            InitializeComponent();
            Validate();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NameTextBox.Text = Environment.UserName;
            NameTextBox.Focus();
            NameTextBox.SelectAll();
        }

        private void Validate()
        {
            CloseBtn.IsEnabled = NameTextBox.Text.Length >= 1;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            PlayerName = NameTextBox.Text;

            Close();
        }

        private void Name_Changed(object sender, TextChangedEventArgs e)
        {
            Validate();
        }

        private void NameTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }
    }
}
