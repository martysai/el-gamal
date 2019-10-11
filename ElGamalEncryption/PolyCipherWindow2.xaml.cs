using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PolynomialLibrary;
using System.ComponentModel;
using System.IO;
using Microsoft.Win32;

namespace ElGamalEncryption
{
    /// <summary>
    /// Вспомогательный класс для вывода формул, на который ссылается DataContext
    /// </summary>
    public class ContextDataCipher2Window : INotifyPropertyChanged
    {
        public string MainFormula { get; set; } // Используются при описании основной формулы
        public string MainDecipherFormula { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string str)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(str));
        }

    }
    
    public partial class PolyCipherWindow2 : Window
    {
        private static int CheckDivisionBy5(int n)
        {
            return (n % 5 == 0) ? 0 : 1;
        }

        public Account Alice, Bob;

        public static List<string> CipheredParts;
        public static List<string> OutputCipheredParts;
        public static List<Polynomial> PolyCipheredParts;
        public static List<string> RelaxedPartes;
        public static List<string> SourceParts;
        public static List<string> EncodedParts;
        public static List<string> DecipheredParts;
        public static List<Polynomial> PolyDecipheredParts;

        public PolyCipherWindow2(Account Alice, Account Bob)
        {
            this.Alice = Alice;
            this.Bob = Bob;
            ContextDataCipher2Window cd = new ContextDataCipher2Window
            {
                MainFormula = @"(t*g^{ab}, g^{a})",
                MainDecipherFormula = @"(t*g^{ab}) * (g^{a})^{-b} = t"
            };
            InitializeComponent();
            DataContext = cd;
            StackPanel sp;
            string message = Data.Message;
            SourceParts = new List<string>(); // Исходные куски сообщения
            EncodedParts = new List<string>(); // Закодированные куски
            CipheredParts = new List<string>(); // Зашифрованные куски
            OutputCipheredParts = new List<string>();
            PolyCipheredParts = new List<Polynomial>();
            DecipheredParts = new List<string>(); // Дешифрованные куски
            RelaxedPartes = new List<string>(); // Зашифрованные в текстовом виде куски 
            PolyDecipheredParts = new List<Polynomial>();

            // В этом цикле создается вся информация выше (т.е. приведение данных к читабельному виду)
            for (int i = 0; i < message.Length / 5 + CheckDivisionBy5(message.Length); i++)
            {
                bool flag = false;
                int size = 5;
                if (message.Length - i * 5 < 5)
                {
                    size = message.Length - i * 5;
                    flag = true;
                }
                int dimOfPrint = (!flag) ? 5 : message.Length % 5; // Какой кусочек нужно восстановить

                SourceParts.Add(message.Substring(i * 5, size));
                Polynomial cash = Data.StringToPolynomial(message.Substring(i * 5, size));
                EncodedParts.Add(MainWindow.PolynomialToString(cash));

                Alice.CreateMessage(message.Substring(i * 5, size));
                Alice.TransportTo(Bob);

                Bob.MessagePolynomial = Bob.Decipher();

                Bob.EncryptedMessage = Data.CheckIt(Bob.EncryptedMessage);

                Polynomial DecipheredPolynomial = Normalize(Bob.Decipher(), dimOfPrint);
                DecipheredParts.Add(MainWindow.PolynomialToString(DecipheredPolynomial));

                CipheredParts.Add(MainWindow.PolynomialToString(Bob.EncryptedMessage));
                OutputCipheredParts.Add(Bob.EncryptedMessage.ToString());
                PolyCipheredParts.Add(Bob.EncryptedMessage);
                RelaxedPartes.Add(GetEncryptedMessage(Bob.EncryptedMessage, flag, message));
                PolyDecipheredParts.Add(Bob.MessagePolynomial);
            }

            // В этом цикле передается кодирование
            for (int i = 0; i < SourceParts.Count; i++)
            {
                sp = new StackPanel { Orientation = Orientation.Horizontal };
                Label med = new Label { Content = SourceParts[i], FontSize = 23 };
                MainWindow.HandleMargin(med, 15);
                MainWindow.AddChildren(sp, med);

                Image myImage = new Image();
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri("Images/Arrow.png", UriKind.Relative);
                bi.EndInit();
                myImage.Source = bi;
                myImage.Width = 50;
                myImage.Height = 50;
                myImage.Stretch = Stretch.Fill;
                MainWindow.HandleMargin(myImage, 20, -6, 20);

                MainWindow.AddChildren(sp, myImage);

                WpfMath.Controls.FormulaControl fml = new WpfMath.Controls.FormulaControl
                {
                    FontSize = 30,
                    Formula = EncodedParts[i] + $"= t_{{{i + 1}}}(x)"
                };
                MainWindow.HandleMargin(fml, 0, 6);
                MainWindow.AddChildren(sp, fml);

                MainWindow.AddChildren(EncodeStackPanel, sp);
            }

            // Создаем общий многочлен для вывода в будущем и для поиска обратного
            Polynomial commonPoly = Data.Modulus(Polynomial.Power(Alice.publicPolynomial, Bob.privatePower,
                Data.FieldOfCryptosystem.Order),
                Data.FieldOfCryptosystem.Modulus, Data.FieldOfCryptosystem.Order);

            Account.CommonKey = MainWindow.PolynomialToString(commonPoly);

            // Здесь шифруются куски
            for (int i = 0; i < EncodedParts.Count; i++)
            {
                sp = new StackPanel { Orientation = Orientation.Horizontal };

                WpfMath.Controls.FormulaControl fml = new WpfMath.Controls.FormulaControl
                {
                    FontSize = 30,
                    Formula = $"t_{{{i + 1}}}(x) = " + EncodedParts[i]
                };
                MainWindow.HandleMargin(fml, 20, 20);
                MainWindow.AddChildren(sp, fml);

                Image myImage = new Image();
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri("Images/Arrow.png", UriKind.Relative);
                bi.EndInit();
                myImage.Source = bi;
                myImage.Width = 50;
                myImage.Height = 50;
                myImage.Stretch = Stretch.Fill;
                MainWindow.HandleMargin(myImage, 20, 6, 20);
                MainWindow.AddChildren(sp, myImage);

                fml = new WpfMath.Controls.FormulaControl
                {
                    FontSize = 30,
                    Formula = $"t_{{{i + 1}}}(x) * g^{{ab}}(x)=({EncodedParts[i]})*({Account.CommonKey})={CipheredParts[i]}" + $"=t'_{i+1}(x)"
                };
                MainWindow.HandleMargin(fml, 0, 16);
                MainWindow.AddChildren(sp, fml);

                MainWindow.AddChildren(TransformStackPanel, sp);
            }

            // Здесь зашифрованные куски идут на вывод в текстовом виде;
            for (int i = 0; i < CipheredParts.Count; i++)
            {
                sp = new StackPanel { Orientation = Orientation.Horizontal };

                WpfMath.Controls.FormulaControl fml = new WpfMath.Controls.FormulaControl
                {
                    FontSize = 30,
                    Formula = $"t'_{i+1}(x) = " + CipheredParts[i]
                };
                MainWindow.HandleMargin(fml, 20, 20);
                MainWindow.AddChildren(sp, fml);

                Image myImage = new Image();
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri("Images/Arrow.png", UriKind.Relative);
                bi.EndInit();
                myImage.Source = bi;
                myImage.Width = 50;
                myImage.Height = 50;
                myImage.Stretch = Stretch.Fill;
                MainWindow.HandleMargin(myImage, 20, 6, 20);
                MainWindow.AddChildren(sp, myImage);

                Label med = new Label { Content = RelaxedPartes[i], FontSize = 20 };
                MainWindow.HandleMargin(med, 0, 14);

                MainWindow.AddChildren(sp, med);
                MainWindow.AddChildren(CipheredStackPanel, sp);
            }
        }

        private static Polynomial Normalize(Polynomial t, int dimOfPrint)
        {
            Polynomial t2;

            int val = dimOfPrint - 1 - t.Dimension;

            if (val > 0)
                t2 = new Polynomial(val - 1);
            else
                t2 = new Polynomial();

            foreach (int lv in t.CoefList)
                t2.Add(lv);

            return t2;
        }

        public static string GetEncryptedMessage(Polynomial p, bool flag, string message)
        {
            int dimOfPrint = (!flag) ? 5 : message.Length % 5; // Какой кусочек нужно восстановить

            int val = dimOfPrint - p.Dimension - 1;

            Polynomial prin;
            if (val > 0)
                prin = new Polynomial(val - 1);
            else
                prin = new Polynomial();

            foreach (int lv in p.CoefList)
            {
                prin.Add(lv);
            }
            return Data.PolynomialToString(prin);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow win = new MainWindow();
            win.Top = Top;
            win.Left = Left;
            win.Show();
            Close();
        }

        private void CipherCloseButton_Click(object sender, RoutedEventArgs e)
        {
            PolyDecipherWindow win = new PolyDecipherWindow(Alice, Bob);
            win.Top = Top;
            win.Left = Left;
            win.Show();
            Close();
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            PolyCipherWindow win = new PolyCipherWindow(Alice, Bob);
            win.Top = Top;
            win.Left = Left;
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

        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (sfd.ShowDialog() == true) // Некоторый формат сохранения в файл
            {
                string content = @"Информация о зашифрованных сообщениях:
";
                int counter = 1;
                for (int i = 0; i < CipheredParts.Count; i++)
                {
                    content += counter++ + " зашифрованный многочлен: " + $"{OutputCipheredParts[i]}" + @"
";
                }
                content += @"~~~~~
";
                counter = 1;
                for (int i = 0; i < RelaxedPartes.Count; i++)
                {
                    content += counter++ + " зашифрованное сообщение: " + $"{Data.PolynomialToString(PolyCipheredParts[i])}" + @"
";
                }
                File.WriteAllText(sfd.FileName, content);
            }
        }
    }
}
