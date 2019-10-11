using System.IO;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Win32;
using System.Text;
using System.Windows.Input;
using PolynomialLibrary;
using System;
using NumberLibrary;
using System.Numerics;

namespace ElGamalEncryption
{
    public partial class ChooseWindow : Window
    {
        public ChooseWindow()
        {
            InitializeComponent();
        }

        private static string message; // Используется сразу в нескольких методах. Проще сохранить

        private static bool IsReadingFromFile; // Флажок, указывающий на то, считываем ли мы с файла

        static ChooseWindow()
        {
            IsReadingFromFile = false;
        }

        public static Account AliceAccount; // Используются в окне PolyCipherWindow
        public static Account BobAccount;
        public static Client AliceClient; // Используются при работе с полем вычетов
        public static Client BobClient;

        /// <summary>
        /// Проверяет число на делимость на 5 (нужно для определения числа операций в ChooseWindow)
        /// </summary>
        private static int CheckDivisionBy5(int n)
        {
            return (n % 5 == 0) ? 0 : 1;
        }
        
        /// <summary>
        /// Метод запускает режим работы с многочленами
        /// </summary>
        private void HandlePolynomialMode(string inputString)
        {
            PolynomialLibrary.Data.CreateCodes(inputString); // Заполним таблицу кодировки
            PolynomialLibrary.Data.FieldOfCryptosystem = new Field(inputString); // Создание общего поля на время выполнения всей программы
            AliceAccount = new Account();
            BobAccount = new Account();

            // Требуется разбиение исходного многочлена на несколько составляющих
            for (int i = 0; i < inputString.Length / 5 + CheckDivisionBy5(inputString.Length); i++)
            {
                int size = 5;
                if (inputString.Length - i * 5 < 5)
                    size = inputString.Length - i * 5;
                AliceAccount.CreateMessage(inputString.Substring(i * 5, size)); // Создается сообщение у Алисы
                AliceAccount.TransportTo(BobAccount);
            }
            PolyDisplayWindow win2 = new PolyDisplayWindow() {
                Top = Top,
                Left = Left
            };
            win2.Show();
            Close();
        }

        /// <summary>
        /// Активизирует режим работы с полем вычетов
        /// </summary>
        private void HandleNumberMode(string inputString)
        {
            NumberLibrary.Data.CreateData(inputString); // Создаем все необходимые характеристики;
            AliceClient = new Client();
            BobClient = new Client();

            NumberInfoWindow win2 = new NumberInfoWindow(AliceClient, BobClient)
            {
                Top = Top,
                Left = Left
            };
            win2.Show();
            Close();
        }
        
        /// <summary>
        /// Обрабатывает событие "Нажатие на клавишу "Зашифровать""
        /// </summary>
        private void Cipher_Click(object sender, RoutedEventArgs e)
        {

            if (!IsReadingFromFile && (DataBox.Text == null || DataBox.Text.Length <= 3) ||
                IsReadingFromFile && (message.Length <= 3 || message == null))
            { // Обработка корректности введенных данных
                ShortMessage win = new ShortMessage
                {
                    Top = Top,
                    Left = Left
                };
                win.Show();
                Close();
                return;
            }

            if (!IsReadingFromFile)
                message = DataBox.Text;

            CheckCapacity(message);

            if (ModeWindow.Mode == 0) // Выбираем используемый режим
                HandleNumberMode(message);
            else
                HandlePolynomialMode(message);
            message = "";
            IsReadingFromFile = false;
        }

        /// <summary>
        /// В данном методе проверяется мощность алфавита
        /// </summary>
        private void CheckCapacity(string input)
        {
            int counter = 0; // в этом счетчике хранится количество различных символов входной строки
                             // Чтобы записать в счетчик хорошее значение, достаточно использовать HashSet
            HashSet<char> cash = new HashSet<char>(input);
            counter = cash.Count;
            int ord = 0;
            PolynomialLibrary.Data.FindPrime(counter, ref ord);
            if (ord > 60)
            {
                CapacityMessage win = new CapacityMessage
                {
                    Top = Top,
                    Left = Left
                };
                win.Show();
                Close();
            }
        }

        /// <summary>
        /// Обработчик события "Нажатия на кнопку "Меню"". Вернемся в главное окно MainWindow
        /// </summary>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            ModeWindow win = new ModeWindow
            {
                Top = Top,
                Left = Left
            };
            win.Show();
            PolynomialLibrary.Data.ClearData(); // Нужно не забыть очистить все данные
            Close();
        }

        /// <summary>
        /// Обработчик события : "Нажатие на клавишу Enter"
        /// </summary>
        private void IsEnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Cipher_Click(sender, e);
            }
        }

        /// <summary>
        /// Для считывания данных из файла
        /// </summary>
        private void FileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
                RestoreDirectory = true
            };
            if (ofd.ShowDialog() == true)
            {
                message = File.ReadAllText(ofd.FileName, Encoding.GetEncoding(1251));
                DataBox.Text = "";
                IsReadingFromFile = true;
                Cipher_Click(sender, e);
            }
        }
    }
}
