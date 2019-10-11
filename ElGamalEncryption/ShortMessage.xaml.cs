using System.Windows;

namespace ElGamalEncryption
{
    public partial class ShortMessage : Window
    {
        public ShortMessage()
        {
            InitializeComponent();
        }
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
