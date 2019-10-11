using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.ComponentModel;
using System.IO;
using System.Threading;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Input;
using PolynomialLibrary;

namespace ElGamalEncryption
{
    /// <summary>
    /// Вспомогательный класс для вывода формул, на который ссылается DataContext
    /// </summary>
    public class ContextDataCipherWindow : INotifyPropertyChanged
    {
        public string A { get; set; } // Закрытая степень Алисы
        public string B { get; set; } // Закрытая степень Боба
        public string Ga { get; set; } // Открытый ключ Алисы
        public string Gb { get; set; } // Открытый ключ Боба
        public string GaPower { get; set; } // Запись (скобочная последовательность) открытого ключа Алисы
        public string GbPower { get; set; } // Запись (скобочная последовательность) открытого ключа Боба
        public string Gab { get; set; } // Общий ключ Алисы и Боба

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string str)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(str));
        }

    }

    /// <summary>
    /// Структура, включающая в себя необходимые свойства для вывода в DataGrid
    /// </summary>
    public struct Item
    {
        public string Symbol { get; set; }
        public string Code { get; set; }
    }
    
    public partial class PolyCipherWindow : Window
    {
        public Account Alice;
        public Account Bob;

        private ObservableCollection<Item> collection = null; // коллекция, хранящая отображение DataGrid

        /// <summary>
        /// Вспомогательный метод для удобчитаемого вывода инфрормации
        /// </summary>
        /// <returns></returns>
        public static string FormPrefix(int key)
        {
            if (key == 1)
                return "g(x) =";
            string p;
            if (key % 2 == 0)
                p = $"g^{{{key}}}(x) = (g^{{{key / 2}}}(x) * g^{{{key / 2}}}(x)";
            else
                p = $"g^{{{key}}}(x) = (g(x)*(g^{{{key / 2}}}(x))^{{{2}}}";
            p += $") mod (h(x))";
            return p;
        }

        

        public PolyCipherWindow(Account A, Account B)
        {
            Alice = A; Bob = B;

            collection = new ObservableCollection<Item>();

            // Вспомогательный класс, хранящий в себе формулы, которые необходимы отобразить
            ContextDataCipherWindow cd = new ContextDataCipherWindow
            {
                A = "a = " + Convert.ToString(Alice.privatePower),
                B = "b = " + Convert.ToString(Bob.privatePower),
                Ga = "g^{a}(x) = " + MainWindow.PolynomialToString(Alice.publicPolynomial),
                Gb = "g^{b}(x) = " + MainWindow.PolynomialToString(Bob.publicPolynomial),
                GaPower = "g^{a}(x) = " + Alice.MessagePowerPolynomial.Replace("(g)", "g"),
                GbPower = "g^{b}(x) = " + Bob.MessagePowerPolynomial.Replace("(g)", "g"),
                Gab = "g^{ab}(x) = " + MainWindow.PolynomialToString(Data.Modulus(Polynomial.Power(Alice.publicPolynomial, Bob.privatePower,
                Data.FieldOfCryptosystem.Order),
                Data.FieldOfCryptosystem.Modulus, Data.FieldOfCryptosystem.Order))
            };
            InitializeComponent();
            DGrid.ItemsSource = collection;

            foreach (KeyValuePair<char, int> pr in Data.codesOfSymbols)
            {
                collection.Add(new Item
                {
                    Symbol = Convert.ToString(pr.Key),
                    Code = Convert.ToString(pr.Value)
                });
            }

            // Теперь необходимо вывести необходимые операции для возведения многочлена в степень
            List<int> AlicePowers = new List<int>(); // Эти степени необходимы, т.к. именно они вычисляются 
            //в методе быстрого возведения в квадрат
            int a = Alice.privatePower;
            while (a > 0)
            {
                AlicePowers.Add(a);
                a /= 2;
            }
            AlicePowers.Reverse();
            Polynomial g = Data.FieldOfCryptosystem.OpenPolynomial;
            int order = Data.FieldOfCryptosystem.Order;
            MainWindow.HandleMargin(AliceStackPanel, 15);
            MainWindow.HandleMargin(BobStackPanel, 15);
            try
            {
                for (int i = 0; i < AlicePowers.Count; i++)
                {
                    string p = FormPrefix(AlicePowers[i]);

                    string pres = MainWindow.PolynomialToString(Alice.BasicPowers[i]);
                    if (AlicePowers[i] % 2 == 0 && AlicePowers[i] != 1) // Нужно сформировать конкретное выражение для вычисления
                        p += $" = (({pres})*({pres})) mod ({MainWindow.PolynomialToString(Data.FieldOfCryptosystem.Modulus)}) = ";
                    else if (AlicePowers[i] != 1)
                        p += $" = (({MainWindow.PolynomialToString(g)})*({pres})^{{2}}) mod ({MainWindow.PolynomialToString(Data.FieldOfCryptosystem.Modulus)}) = ";
                    p += MainWindow.PolynomialToString(Bob.BasicPowers[i + 1]); // Добавим в конец результат, чтобы узнать, чему равняется текущая степень
                    Thread.Sleep(300);
                    WpfMath.Controls.FormulaControl f = new WpfMath.Controls.FormulaControl()
                    {
                        Formula = p
                    };
                    Thread.Sleep(300);
                    MainWindow.AddChildren(AliceStackPanel, f);
                }
                List<int> BobPowers = new List<int>();// Эти степени необходимы, т.к. именно они вычисляются 
                                                      //в методе быстрого возведения в квадрат
                int b = Bob.privatePower;
                while (b > 0)
                {
                    BobPowers.Add(b);
                    b /= 2;
                }
                BobPowers.Reverse();
                for (int i = 0; i < BobPowers.Count; i++)
                {
                    string p = FormPrefix(BobPowers[i]);

                    string pres = MainWindow.PolynomialToString(Bob.BasicPowers[i]);
                    if (BobPowers[i] % 2 == 0 && BobPowers[i] != 1) // Нужно сформировать конкретное выражение для вычисления
                        p += $" = (({pres})*({pres})) mod ({MainWindow.PolynomialToString(Data.FieldOfCryptosystem.Modulus)}) = ";
                    else if (BobPowers[i] != 1)
                        p += $" = (({MainWindow.PolynomialToString(g)})*({pres})^{{2}}) mod ({MainWindow.PolynomialToString(Data.FieldOfCryptosystem.Modulus)}) = ";
                    p += MainWindow.PolynomialToString(Bob.BasicPowers[i + 1]); // Добавим в конец результат, чтобы узнать, чему равняется текущая степень
                    Thread.Sleep(300);
                    WpfMath.Controls.FormulaControl f = new WpfMath.Controls.FormulaControl()
                    {
                        Formula = p
                    };
                    Thread.Sleep(300);
                    MainWindow.AddChildren(BobStackPanel, f);
                }
            } catch (Exception)
            {
                PolyDisplayWindow win = new PolyDisplayWindow()
                {
                    Top = Top,
                    Left = Left
                };
            }
            DataContext = cd;
        }

        /// <summary>
        /// Метод-обработчик используется для вызова нового окна
        /// </summary>
        private void CipherCloseButton_Click(object sender, RoutedEventArgs e)
        {
            PolyCipherWindow2 win = new PolyCipherWindow2(Alice, Bob) {
                Top = Top,
                Left = Left
            };
            win.Show();
            Close();
        }

        /// <summary>
        /// Используется для обработки события "Нажатие на кнопку "Сохранить"".
        /// </summary>
        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (sfd.ShowDialog() == true) // Некоторый формат сохранения в файл
            {
                string content = @"Информация об Алисе:
"
                + $"Открытый многочлен: {Alice.publicPolynomial.ToString()}" + @"
"
                + $"Закрытая степень: {Alice.privatePower.ToString()}" + @"
"
                + @"Информация о Бобе:
"
                + $"Открытый многочлен: {Bob.publicPolynomial.ToString()}" + @"
"
                + $"Закрытая степень: {Bob.privatePower.ToString()}";
                File.WriteAllText(sfd.FileName, content);
            }
        }

        /// <summary>
        /// Обработчик события "Нажатие на кнопку "Меню"".
        /// </summary>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow win = new MainWindow() { 
                Top = Top,
                Left = Left
            };
            win.Show();
            Close();
        }

        /// <summary>
        /// Обработчик события "Нажатие на кнопку "Назад"".
        /// </summary>
        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            PolyDisplayWindow win = new PolyDisplayWindow()
            {
                Top = Top,
                Left = Left
            };
            win.Show();
            Close();
        }

        private void IsEnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                CipherCloseButton_Click(sender, e);
            }
        }
    }
}
