using System;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using PolynomialLibrary;
using WpfMath;

using System.Globalization;
using System.Windows.Markup;

namespace ElGamalEncryption
{
    /// <summary>
    /// Вспомогательный класс для вывода формул, на который ссылается DataContext
    /// </summary>
    public class ContextData : INotifyPropertyChanged
    {
        public string G { get; set; }
        public string dG { get; set; }
        public string H { get; set; }
        public string dH { get; set; }
        public string p { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string str)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(str));
        }

    }
    
    public partial class PolyDisplayWindow : Window
    {
        public PolyDisplayWindow()
        {
            InitializeComponent();
            ContextData cd = new ContextData()
            {
                G = "g(x) = " + MainWindow.PolynomialToString(Data.FieldOfCryptosystem.OpenPolynomial),
                dG = $"deg(g) ={ Data.FieldOfCryptosystem.OpenPolynomial.Dimension }",
                H = "h(x) = " + MainWindow.PolynomialToString(Data.FieldOfCryptosystem.Modulus),
                dH = "deg(h)=5",
                p = "p = " + Data.FieldOfCryptosystem.Order,
            };
            WpfMath.Controls.FormulaControl fml;
            StackPanel sp;
            Label med;


            // Вывод даных о генерации неприводимого многочлена
            for (int i = 0; i < Data.OutputPolynomialList.Count; i++)
            {
                sp = new StackPanel() { Orientation = Orientation.Horizontal };

                fml = new WpfMath.Controls.FormulaControl
                {
                    Formula = MainWindow.PolynomialToString(Data.OutputPolynomialList[i].Key),
                };
                MainWindow.HandleMargin(fml, 17, 17);
                sp.Children.Add(fml);

                med = new Label
                {
                    FontFamily = new FontFamily("Times New Roman"),
                    FontSize = 19,
                    Content = "делится на "
                };
                MainWindow.HandleMargin(med, 10, 12);
                sp.Children.Add(med);

                fml = new WpfMath.Controls.FormulaControl
                {
                    Formula = MainWindow.PolynomialToString(Data.OutputPolynomialList[i].Value),
                };
                MainWindow.HandleMargin(fml, 20, 21);
                sp.Children.Add(fml);

                MainStackPanel.Children.Add(sp); // Конструируем стек-панель через вспомогательную
            }

            // Необходимо обработать пойманный результат (неприводимый многочлен)

            sp = new StackPanel() { Orientation = Orientation.Horizontal };
            fml = new WpfMath.Controls.FormulaControl
            {
                Formula = MainWindow.PolynomialToString(Data.FieldOfCryptosystem.Modulus)
            };
            MainWindow.HandleMargin(fml, 17, 17);
            sp.Children.Add(fml);

            med = new Label
            {
                FontFamily = new FontFamily("Times New Roman"),
                FontSize = 20,
                Content = string.Format("является неприводимым многочленом над полем порядка {0}"
                , Data.FieldOfCryptosystem.Order),
                Foreground = Brushes.Red
            };
            MainWindow.HandleMargin(med, 10, 10);
            sp.Children.Add(med);
            MainStackPanel.Children.Add(sp);

            DataContext = cd; // Присвоим контекст , чтобы XAML мог обрабатывать формулы
        }

        private void FieldCloseButton_Click(object sender, RoutedEventArgs e)
        {
            PolyCipherWindow win = new PolyCipherWindow(ChooseWindow.AliceAccount, ChooseWindow.BobAccount);
            win.Top = Top;
            win.Left = Left;
            win.Show();
            Close();
        }

        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (sfd.ShowDialog() == true) // Некоторый формат сохранения в файл
            {
                string content = @"Информация о поле:
"
                + $"Открытый многочлен: {Data.FieldOfCryptosystem.OpenPolynomial.ToString()}" + @"
"
                + $"Неприводимый многочлен: {Data.FieldOfCryptosystem.Modulus.ToString()}" + @"
"
                + $"Порядок поля вычетов: {Data.FieldOfCryptosystem.Order.ToString()}";
                File.WriteAllText(sfd.FileName, content);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NumberLibrary.Data.ClearData();
            PolynomialLibrary.Data.ClearData();
            MainWindow win = new MainWindow() {
                Top = Top,
                Left = Left
            };
            win.Show();
            Close();
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
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
