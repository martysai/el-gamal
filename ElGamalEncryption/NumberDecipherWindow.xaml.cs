using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.IO;
using Microsoft.Win32;
using System.Numerics;
using NumberLibrary;

namespace ElGamalEncryption
{
    /// <summary>
    /// Вспомогательный класс для вывода формул, на который ссылается DataContext
    /// </summary>
    public class ContextDataNumberDecipherWindow : INotifyPropertyChanged
    {
        public string Gabo { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string str)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(str));
        }

    }

    public partial class NumberDecipherWindow : Window
    {
        public Client Alice, Bob;

        public NumberDecipherWindow(Client Alice, Client Bob)
        {
            this.Alice = Alice;
            this.Bob = Bob;

            

            // Инициализируем необходимы DataContext;
            ContextDataNumberDecipherWindow cd = new ContextDataNumberDecipherWindow
            {
                Gabo = @"g^{-ab}(x)=" + $"({Client.CommonKey})^{{-1}}={Client.InversedCommonKey}"
            };


            InitializeComponent();
            DataContext = cd;

            StackPanel sp;

            // Кодирование зашифрованных текстовых кусочков
            for (int i = 0; i < NumberCipherWindow.CipheredParts.Count; i++)
            {
                sp = new StackPanel { Orientation = Orientation.Horizontal };

                Label med = new Label { Content = Data.Decode(BigInteger.Parse(NumberCipherWindow.CipheredParts[i])), FontSize = 20 };
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
                    Formula = NumberCipherWindow.CipheredParts[i] + $" = t'_{i + 1}(x)"
                };
                MainWindow.HandleMargin(fml, 0, 7);
                MainWindow.AddChildren(sp, fml);
                MainWindow.AddChildren(EncodeStackPanel, sp);
            }

            for (int i = 0; i < NumberCipherWindow.EncodedParts.Count; i++)
            {
                sp = new StackPanel { Orientation = Orientation.Horizontal };

                WpfMath.Controls.FormulaControl fml = new WpfMath.Controls.FormulaControl
                {
                    FontSize = 30,
                    Formula = $"t'_{i + 1}(x) = " + NumberCipherWindow.CipheredParts[i]
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
                
                fml = new WpfMath.Controls.FormulaControl
                {
                    FontSize = 30,
                    Formula = $"(t'_{{{i+1}}}*g^{{-ab}})mod(p)=({NumberCipherWindow.CipheredParts[i]}*{Client.InversedCommonKey}) mod ({Data.Modulus})={NumberCipherWindow.DecipheredParts[i]}=t_{{{i + 1}}}(x)"
                };
                MainWindow.HandleMargin(fml, 0, 18);
                MainWindow.AddChildren(sp, fml);

                MainWindow.AddChildren(DecipherStackPanel, sp);
            }

            // Восстановление дешифрованных кусков
            for (int i = 0; i < NumberCipherWindow.SourceParts.Count; i++)
            {
                sp = new StackPanel { Orientation = Orientation.Horizontal };

                WpfMath.Controls.FormulaControl fml = new WpfMath.Controls.FormulaControl
                {
                    FontSize = 30,
                    Formula = $"t_{{{i + 1}}}(x) = " + NumberCipherWindow.EncodedParts[i]
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
                Label med = new Label { Content = NumberCipherWindow.SourceParts[i], FontSize = 23 };
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
            { // Некоторый формат сохранения в файл
                string content = @"Информация о востановленных сообщениях:
";
                int counter = 1;
                for (int i = 0; i < NumberCipherWindow.EncodedParts.Count; i++)
                {
                    content += counter++ + " востановленный кусок: " + $"{NumberCipherWindow.EncodedParts[i].ToString()}" + @"
";
                }

                File.WriteAllText(sfd.FileName, content);
            }
            else
                MessageBox.Show("Произошла ошибка сохранения.");
        }
    }
}
