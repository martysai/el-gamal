using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PolynomialLibrary;
using System.ComponentModel;
using System.IO;
using Microsoft.Win32;
using NumberLibrary;
using System.Numerics;

namespace ElGamalEncryption
{
    /// <summary>
    /// Вспомогательный класс для вывода формул, на который ссылается DataContext
    /// </summary>
    public class ContextDataNumberCipherWindow : INotifyPropertyChanged
    {
        public string MainFormula { get; set; } // Используются при описании основной формулы
        public string MainDecipherFormula { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string str)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(str));
        }

    }

    public partial class NumberCipherWindow : Window
    {
        private static int CheckDivisionBy5(int n)
        {
            return (n % 5 == 0) ? 0 : 1;
        }

        public Client Alice, Bob;

        public static List<string> CipheredParts;
        public static List<string> RelaxedPartes;
        public static List<string> SourceParts;
        public static List<string> EncodedParts;
        public static List<string> DecipheredParts;

        public NumberCipherWindow(Client Alice, Client Bob)
        {
            this.Alice = Alice;
            this.Bob = Bob;
            ContextDataNumberCipherWindow cd = new ContextDataNumberCipherWindow
            {
                MainFormula = @"(t*g^{ab}, g^{a})",
                MainDecipherFormula = @"(t*g^{ab}) * (g^{a})^{-b} = t"
            };
            InitializeComponent();
            DataContext = cd;
        
            StackPanel sp;
            string message = NumberLibrary.Data.Message;
            SourceParts = new List<string>(); // Исходные куски сообщения
            EncodedParts = new List<string>(); // Закодированные куски
            CipheredParts = new List<string>(); // Зашифрованные куски
            DecipheredParts = new List<string>(); // Дешифрованные куски
            RelaxedPartes = new List<string>(); // Зашифрованные в текстовом виде куски 

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

                SourceParts.Add(message.Substring(i * 5, size)); // Исходные куски
                BigInteger cash = NumberLibrary.Data.Encode(message.Substring(i * 5, size));
                EncodedParts.Add(cash.ToString()); // Закодированные куски

                Alice.SentMessage = message.Substring(i * 5, size);
                Alice.TransportTo(Bob);

                Bob.Decipher(); // Выполним дешифрование
                CipheredParts.Add(Bob.CipheredNumber.ToString());
                DecipheredParts.Add(NumberLibrary.Data.Encode(Bob.RecievedMessage).ToString()); // Дешифрованные куски
                RelaxedPartes.Add(Bob.RecievedMessage);
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
                    Formula = NumberLibrary.Data.EncodeAndConvert(SourceParts[i]) + $"= {EncodedParts[i]} = t_{{{i+1}}}"
                };
                MainWindow.HandleMargin(fml, 0, 8);
                MainWindow.AddChildren(sp, fml);

                MainWindow.AddChildren(EncodeStackPanel, sp);
            }

            // Здесь шифруются куски
            for (int i = 0; i < EncodedParts.Count; i++)
            {
                sp = new StackPanel { Orientation = Orientation.Horizontal };

                WpfMath.Controls.FormulaControl fml = new WpfMath.Controls.FormulaControl
                {
                    FontSize = 30,
                    Formula = $"t_{{{i + 1}}} =" + EncodedParts[i]
                };
                MainWindow.HandleMargin(fml, 20, 22);
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
                MainWindow.HandleMargin(myImage, 20, 5, 20);
                MainWindow.AddChildren(sp, myImage);

                fml = new WpfMath.Controls.FormulaControl
                {
                    FontSize = 30,
                    Formula = $"(t_{{{i + 1}}} * g^{{ab}})mod(p)=({EncodedParts[i]}*{Client.CommonKey}) mod ({NumberLibrary.Data.Modulus})={CipheredParts[i]}=t'_{{{i + 1}}}"
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
                    Formula = $"t'_{{{i + 1}}} = " + CipheredParts[i]
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

                Label med = new Label { Content = NumberLibrary.Data.Decode(BigInteger.Parse(CipheredParts[i])), FontSize = 20 };
                MainWindow.HandleMargin(med, 0, 12);

                MainWindow.AddChildren(sp, med);
                MainWindow.AddChildren(CipheredStackPanel, sp);
            }

            Exchange();
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
            NumberDecipherWindow win = new NumberDecipherWindow(Alice, Bob);
            win.Top = Top;
            win.Left = Left;
            win.Show();
            Close();
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            NumberInfoWindow win = new NumberInfoWindow(Alice, Bob);
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

        private void Exchange()
        {
            for (int i = 0; i < DecipheredParts.Count; i++)
                DecipheredParts[i] = EncodedParts[i];
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
                    content += counter++ + $" зашифрованное сообщение t({i + 1}): " + $"{CipheredParts[i].ToString()}" + @"
";
                }
                File.WriteAllText(sfd.FileName, content);
            }
        }
    }
}
