using System.Windows;

namespace ElGamalEncryption
{
    public partial class ModeWindow : Window
    {
        public static int Mode { get; set; } // Выбираемый пользователем режим работы

        public ModeWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик события "Нажатия на кнопку "Меню"". Вернемся в главное окно MainWindow
        /// </summary>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow win = new MainWindow
            {
                Top = Top,
                Left = Left
            };
            win.Show();
            Close();
        }

        /// <summary>
        /// При выборе режима работы с многочленами вызывается этот метод.
        /// </summary>
        private void PolynomialButton_Click(object sender, RoutedEventArgs e)
        {
            Mode = 1;
            ChooseWindow win = new ChooseWindow()
            {
                Top = Top,
                Left = Left
            };
            win.Show();
            Close();
        }

        /// <summary>
        /// Для работы с вычетами вызывается этот метод
        /// </summary>
        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            Mode = 0;
            ChooseWindow win = new ChooseWindow()
            {
                Top = Top,
                Left = Left
            };
            win.Show();
            Close();
        }
    }
}
