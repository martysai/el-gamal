using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.ComponentModel;
using System.IO;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Input;
using NumberLibrary;
using System.Numerics;

namespace ElGamalEncryption
{
    /// <summary>
    /// Класс-обертка над информацией, необходимой для вывода в окне
    /// </summary>
    public class ContextDataNumberInfo : INotifyPropertyChanged
    {
        public string A { get; set; } // Закрытая степень Алисы
        public string B { get; set; } // Закрытая степень Боба
        public string Ga { get; set; } // Открытый ключ Алисы
        public string Gb { get; set; } // Открытый ключ Боба
        public string Gab { get; set; } // Общий ключ ребят
        public string p { get; set; } // Порядок поля вычетов
        public string g { get; set; } // открытое значени
        // Эта информация превращается из строкового представления в формулу Latex

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string str)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(str));
        }

    }
    
    public partial class NumberInfoWindow : Window
    {
        public Client Alice; // Для гармоничного использования методов требуется сохранить данные о каждом клиенте
        public Client Bob;

        private ObservableCollection<Item> collection = null; // коллекция, хранящая отображение DataGrid

        public static string FormPrefix(int key)
        {
            if (key == 1)
                return "g =";
            string p;
            if (key % 2 == 0)
                p = $"g^{{{key}}} = (g^{{{key / 2}}} * g^{{{key / 2}}}"; // Добавляем дополнительную скобку для префикса
            else
                p = $"g^{{{key}}} = (g*(g^{{{key / 2}}})^{{{2}}}";
            p += $") mod (p)";
            return p;
        }



        public NumberInfoWindow(Client A, Client B)
        {
            Alice = A; Bob = B;
            Client.CreateCommonKey(Bob.OpenNumber, Alice.PrivatePower);
            Client.CreateInversedCommonKey();

            collection = new ObservableCollection<Item>();
            // Инициализируем необходимы DataContext;
            ContextDataNumberInfo cd = new ContextDataNumberInfo
            {
                A = "a = " + Convert.ToString(Alice.PrivatePower),
                B = "b = " + Convert.ToString(Bob.PrivatePower),
                Ga = "g^{a} = " + Alice.OpenNumber,
                Gb = "g^{b} = " + Bob.OpenNumber,
                Gab = "g^{ab} = " + Client.CommonKey,
                p = "p = " + Data.Modulus,
                g = "g = " + Data.PublicNumber
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

            List<int> AlicePowers = new List<int>();
            int a = Alice.PrivatePower;
            while (a > 0)
            {
                AlicePowers.Add(a);
                a /= 2;
            }
            AlicePowers.Reverse();
            for (int i = 0; i < AlicePowers.Count; i++)
            {
                string p = FormPrefix(AlicePowers[i]); // Начальный префикс выводимой строки

                string pres = Alice.BasicPowers[i].ToString();
                if (AlicePowers[i] % 2 == 0 && AlicePowers[i] != 1) // Добавляем некоторый префикс перед выводом
                    p += $" = (({pres})*({pres})) mod ({Data.Modulus.ToString()}) = ";
                else if (AlicePowers[i] != 1)
                    p += $" = (({Data.PublicNumber.ToString()}))*({pres})^{{2}}) mod ({Data.Modulus.ToString()}) = ";
                p += Alice.BasicPowers[i + 1].ToString(); // Добавить самое значение выражения
                WpfMath.Controls.FormulaControl fc = MainWindow.Formula(p);
                MainWindow.HandleMargin(fc, 15);
                MainWindow.AddChildren(AliceStackPanel, fc);
            }

            List<int> BobPowers = new List<int>();
            int b = Bob.PrivatePower;
            while (b > 0)
            {
                BobPowers.Add(b);
                b /= 2;
            }
            BobPowers.Reverse();
            for (int i = 0; i < BobPowers.Count; i++)
            {
                string p = FormPrefix(BobPowers[i]);

                string pres = Bob.BasicPowers[i].ToString();
                if (BobPowers[i] % 2 == 0 && BobPowers[i] != 1) // Необходимый префикс для удобчитаемого вида
                    p += $" = (({pres})*({pres})) mod ({Data.Modulus}) = ";
                else if (BobPowers[i] != 1)
                    p += $" = (({Data.PublicNumber})*({pres})^{{2}}) mod ({Data.Modulus}) = ";
                p += Bob.BasicPowers[i + 1]; // Добавить самое значение выражения
                WpfMath.Controls.FormulaControl fc = MainWindow.Formula(p);
                MainWindow.HandleMargin(fc, 15);
                MainWindow.AddChildren(BobStackPanel, fc);
            }

            DataContext = cd;
        }

        private void FieldCloseButton_Click(object sender, RoutedEventArgs e)
        {
            NumberCipherWindow win = new NumberCipherWindow(Alice, Bob)
            {
                Top = Top,
                Left = Left
            };
            win.Show();
            Close();
        }

        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (sfd.ShowDialog() == true)
            {
                string content = $"Порядок поля: {Data.Modulus}" + @"
";
                content += @"Информация об Алисе:
"
                + $"Открытый ключ: {Alice.OpenNumber}" + @"
"
                + $"Закрытая степень: {Alice.PrivatePower}" + @"
"
                + @"Информация о Бобе:
"
                + $"Открытый ключ: {Bob.OpenNumber}" + @"
"
                + $"Закрытая степень: {Bob.PrivatePower}";
                File.WriteAllText(sfd.FileName, content);
            }
        }

        /// <summary>
        /// Обработчик события "Нажатия на кнопку "Меню"". Вернемся в главное окно MainWindow
        /// </summary>
        private void BackButton_Click(object sender, RoutedEventArgs e)
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
        /// Обработчик события "Нажатие на кнопку "Назад"". Вернемся в поле ввода текста
        /// </summary>
        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            NumberLibrary.Data.ClearData();
            PolynomialLibrary.Data.ClearData();
            ChooseWindow win = new ChooseWindow()
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
                FieldCloseButton_Click(sender, e);
            }
        }
    }
}
