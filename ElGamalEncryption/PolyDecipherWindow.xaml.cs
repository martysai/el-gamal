using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.IO;
using Microsoft.Win32;
using PolynomialLibrary;

namespace ElGamalEncryption
{
    /// <summary>
    /// Вспомогательный класс для вывода формул, на который ссылается DataContext
    /// </summary>
    public class ContextDataDecipherWindow : INotifyPropertyChanged
    {
        public string Gabo { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string str)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(str));
        }

    }
    
    public partial class PolyDecipherWindow : Window
    {
        public static Polynomial ConvertToNormalView(Polynomial t, int dimOfPrint)
        {
            Polynomial t2;

            int val = dimOfPrint - 1 - t.Dimension;

            if (val > 0)
                t2 = new Polynomial(val - 1);
            else
                t2 = new Polynomial();

            foreach (int lv in t.CoefList)
            {
                t2.Add(lv);
            }
            return t2;
        }

        public Account Alice, Bob;

        public PolyDecipherWindow(Account Alice, Account Bob)
        {
            this.Alice = Alice;
            this.Bob = Bob;

            // Обратный многочлен, на который умножается зашифрованное слово
            string invRepresentation = MainWindow.PolynomialToString(Account.InversedPower);

            // Инициализируем необходимы DataContext;
            ContextDataDecipherWindow cd = new ContextDataDecipherWindow
            {
                Gabo = @"g^{-ab}(x)=" + $"({Account.CommonKey})^{{-1}}={invRepresentation}"
            };


            InitializeComponent();
            DataContext = cd;

            StackPanel sp;

            // Кодирование зашифрованных текстовых кусочков
            for (int i = 0; i < PolyCipherWindow2.CipheredParts.Count; i++)
            {
                sp = new StackPanel { Orientation = Orientation.Horizontal };

                Label med = new Label { Content = PolyCipherWindow2.RelaxedPartes[i], FontSize = 20 };
                MainWindow.HandleMargin(med, 20);
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
                MainWindow.HandleMargin(myImage, 20, -7, 20);

                MainWindow.AddChildren(sp, myImage);

                WpfMath.Controls.FormulaControl fml = new WpfMath.Controls.FormulaControl
                {
                    FontSize = 30,
                    Formula = PolyCipherWindow2.CipheredParts[i] + $" = t'_{i + 1}(x)"
                };
                MainWindow.HandleMargin(fml, 0, 7);
                MainWindow.AddChildren(sp, fml);
                MainWindow.AddChildren(EncodeStackPanel, sp);
            }

            for (int i = 0; i < PolyCipherWindow2.EncodedParts.Count; i++)
            {
                sp = new StackPanel { Orientation = Orientation.Horizontal };

                WpfMath.Controls.FormulaControl fml = new WpfMath.Controls.FormulaControl
                {
                    FontSize = 30,
                    Formula = $"t'_{i + 1}(x) = " + PolyCipherWindow2.CipheredParts[i]
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
                MainWindow.HandleMargin(myImage, 20, 5, 20);
                MainWindow.AddChildren(sp, myImage);

                string md = MainWindow.PolynomialToString(Data.FieldOfCryptosystem.Modulus);
                fml = new WpfMath.Controls.FormulaControl
                {
                    FontSize = 30,
                    Formula = $"(({PolyCipherWindow2.CipheredParts[i]})*({invRepresentation})) mod ({md})={PolyCipherWindow2.DecipheredParts[i]}=t_{{{i + 1}}}(x)"
                };
                MainWindow.HandleMargin(fml, 0, 18);
                MainWindow.AddChildren(sp, fml);

                MainWindow.AddChildren(DecipherStackPanel, sp);
            }

            // Восстановление дешифрованных кусков
            for (int i = 0; i < PolyCipherWindow2.SourceParts.Count; i++)
            {
                sp = new StackPanel { Orientation = Orientation.Horizontal };

                WpfMath.Controls.FormulaControl fml = new WpfMath.Controls.FormulaControl
                {
                    FontSize = 30,
                    Formula = $"t_{{{i + 1}}}(x) = " + PolyCipherWindow2.EncodedParts[i]
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
                MainWindow.HandleMargin(myImage, 20, 4, 20);
                MainWindow.AddChildren(sp, myImage);
                Label med = new Label { Content = PolyCipherWindow2.SourceParts[i], FontSize = 23 };
                MainWindow.HandleMargin(med, 0, 8);
                MainWindow.AddChildren(sp, med);
                
                MainWindow.AddChildren(DecodeStackPanel, sp);
            }

            MainWindow.AddChildren(MainPanel, new Label { FontSize = 25, Content = Data.Message });
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow win = new MainWindow();
            win.Top = Top;
            win.Left = Left;
            win.Show();
            Close();
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            PolyCipherWindow2 win = new PolyCipherWindow2(Alice, Bob) {
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
            { // Некоторый формат сохранения в файл
                string content = @"Информация о востановленных сообщениях:
";
                int counter = 1;
                for (int i = 0; i < PolyCipherWindow2.EncodedParts.Count; i++)
                {
                    content += counter++ + " востановленный многочлен: " + $"{Data.StringToPolynomial(PolyCipherWindow2.SourceParts[i]).ToString()}" + @"
";
                }
                content += @"~~~~~
";
                counter = 1;
                for (int i = 0; i < PolyCipherWindow2.SourceParts.Count; i++)
                {
                    content += counter++ + " востановленное сообщение: " + $"{PolyCipherWindow2.SourceParts[i].ToString()}" + @"
";
                }
                File.WriteAllText(sfd.FileName, content);
            }
        }
    }
}
