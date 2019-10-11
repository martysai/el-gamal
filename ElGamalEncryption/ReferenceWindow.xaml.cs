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
    public class ReferenceContextData : INotifyPropertyChanged
    {
        public string FxF { get; set; } // Latex-формулы некоторых величин в справке
        public string Fpm { get; set; }
        public string CommutableAddition { get; set; }
        public string ModP { get; set; }
        public string CommutableMultiplication { get; set; }
        public string AssosiativeAddition { get; set; }
        public string Pn { get; set; }
        public string AssociativeMultiplication { get; set; }
        public string F { get; set; }
        public string Ai { get; set; }
        public string N { get; set; }
        public string P { get; set; }
        public string EncodeFirstMode { get; set; }
        public string M { get; set; }
        public string H { get; set; }
        public string One { get; set; }
        public string Zero { get; set; }
        public string Pedegree { get; set; }
        public string Minus { get; set; }
        public string Plusmult { get; set; }
        public string Back { get; set; }
        public string Distribute { get; set; }
        public string MainFormula { get; set; }
        public string MainDecipherFormula { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string str)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(str));
        }

    }

    public partial class ReferenceWindow : Window
    {
        public ReferenceWindow()
        {
            InitializeComponent();
            ReferenceContextData cd = new ReferenceContextData()
            {
                FxF = @"M\times M \rightarrow M",
                Fpm = @"\langle F,+,* \rangle",
                CommutableAddition = @"\forall a,b\in F: a+b=b+a",
                CommutableMultiplication = @"\forall a,b\in F a*b=b*a: \forall a,b\in F: a*b=b*a",
                F = "F",
                Plusmult = "+, *",
                AssosiativeAddition = @"\forall a,b,c\in F: (a+b)+c=a+(b+c)",
                AssociativeMultiplication = @"\forall a,b,c\in F: (a*b)*c=a*(b*c)",
                Zero = @"\exists 0\in F: \forall a\in F: a+0=a",
                Minus = @"\forall a\in F:\exists (-a)\in F: (-a)+a=0",
                One = @"\exists e\in F\setminus {0}: \forall a\in F: a*e=a",
                Back = @"\forall a\in F: a\neq 0: \exists a^{-1}\in F: a*a^{-1}=e",
                Distribute = @"\forall a,b,c\in F: (a+b)*c=(a*c)+(b*c)",
                ModP = "a_i = ((n) mod (p^{i+1})) / p^{i}",
                P = "p",
                N = "n",
                H = "h(x)",
                Pn = "p^n",
                M = "M",
                EncodeFirstMode = "a_4 + a_3 * M + a_2 * M^2 + a_1 * M^3 + a_0 * M^4",
                Ai = "a_i",
                Pedegree = "p^{deg(h)}",
                MainFormula = @"(t*g^{ab}, g^{a})",
                MainDecipherFormula = @"(t*g^{ab}) * (g^{a})^{-b} = t"
            };
            DataContext = cd; // Присвоим контекст , чтобы XAML мог обрабатывать формулы
        }
        
        private void IsEnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                MenuButton_Click(sender, e);
            }
        }

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
    }
}
