using System.Windows;

namespace ElGamalEncryption
{
    public partial class CapacityMessage : Window
    {
        public CapacityMessage()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Обработка события "Нажатие на кнопку "Повторить ввод""
        /// </summary>
        private void RepeatInputButton_Click(object sender, RoutedEventArgs e)
        {
            ChooseWindow win = new ChooseWindow();
            win.Top = Top;
            win.Left = Left;
            win.Show();
            Close();
        }
    }
}
