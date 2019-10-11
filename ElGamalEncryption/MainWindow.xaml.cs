using System;
using System.Windows;
using System.Windows.Controls;
using PolynomialLibrary;
using NumberLibrary;

namespace ElGamalEncryption
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик события "Нажатия на кнопку "Начать"". Начинается обработка данных
        /// </summary>
        public void StartButton_Click(object sender, EventArgs e)
        {
            NumberLibrary.Data.ClearData();
            PolynomialLibrary.Data.ClearData();
            ModeWindow win = new ModeWindow() { Top = Top, Left = Left };
            win.Show();
            Close();
        }

        /// <summary>
        /// Аналогичный метод содержится в классе PolynomialLibrary.Data;
        /// Отличие заключается в том, что здесь происходит конвертация в формат LaTeX
        /// </summary>
        /// <param name="p">Входной многочлен</param>
        public static string PolynomialToString(Polynomial p)
        {
            if (p.CoefList.Count == 0)
                return "0";
            string res = "";
            for (int i = 0; i < p.Dimension; i++)
            {
                if (p[i] == 0)
                    continue;
                string digit = (p[i] != 1) ? Convert.ToString(p[i]) : ""; // Считаем цифру, которую нужно вставить
                if (p.Dimension - i != 1)
                    res += $"{digit}x^{{{p.Dimension - i}}}+";
                else
                    res += $"{digit}x+";
            }
            if (p[p.Dimension] != 0) // Последнюю цифру, если она является нулем, писать не нужно
                res += Convert.ToString(p[p.Dimension]);
            else
                res = res.Substring(0, res.Length - 1);
            return res;
        }

        /// <summary>
        /// Удобный метод, изменяющий Margin некоторого экземпляра FrameworkElement
        /// </summary>
        /// <param name="c">Элемент WPF</param>
        public static void HandleMargin(FrameworkElement c, int l = 0, int t = 0, int r = 0, int b = 0)
        {
            Thickness mgn = c.Margin;
            mgn.Left = l;
            mgn.Top = t;
            mgn.Right = r;
            mgn.Bottom = b;
            c.Margin = mgn;
        }

        /// <summary>
        /// Метод для сокращения записи sp.Children.Add(ui)
        /// </summary>
        public static void AddChildren(StackPanel sp, UIElement ui)
        {
            sp.Children.Add(ui);
        }

        /// <summary>
        /// Метод для сокращения записи new WpfMath.Controls.FormulaControl { Formula = s };
        /// </summary>
        public static WpfMath.Controls.FormulaControl Formula(string s)
        {
            return new WpfMath.Controls.FormulaControl { Formula = s };
        }

        private void ReferenceButton_Click(object sender, RoutedEventArgs e)
        {
            ReferenceWindow win = new ReferenceWindow()
            {
                Top = Top,
                Left = Left
            };
            win.Show();
            Close();
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow win = new AboutWindow()
            {
                Top = Top,
                Left = Left
            };
            win.Show();
            Close();
        }
    }
}
