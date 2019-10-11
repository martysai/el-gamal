using System;
using System.Windows;
using System.Windows.Input;

namespace ElGamalEncryption
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработка события "Нажатие на клавишу "В меню"".
        /// </summary>
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow win = new MainWindow()
            {
                Top = Top,
                Left = Left
            };
            win.Show();
            Close();
        }

        /// <summary>
        /// Обработка события "Нажатие на клавишу "Enter""
        /// </summary>
        private void IsEnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                MenuButton_Click(sender, e);
            }
        }
    }
}
