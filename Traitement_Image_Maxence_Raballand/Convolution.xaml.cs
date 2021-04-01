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

namespace Traitement_Image_Maxence_Raballand
{
    /// <summary>
    /// Logique d'interaction pour Convolution.xaml
    /// </summary>
    public partial class Convolution : Window
    {
        TextBox[,] kernel;
        MainWindow window;

        public Convolution(MainWindow window)
        {
            this.window = window;
            kernel = new TextBox[5, 5];
            InitializeComponent();
        }
        private void IntOrNull(object sender, TextCompositionEventArgs e)
        {
            char c = Convert.ToChar(e.Text);
            TextBox txtBox = (TextBox)sender;
            if ((Char.IsNumber(c) || c=='-' || c=='.') && txtBox.Text.Length < 5)
                e.Handled = false;
            else
                e.Handled = true;

            base.OnPreviewTextInput(e);
        }

        private void Validate(object sender, RoutedEventArgs e)
        {
            kernel[0, 0] = c00;
            kernel[0, 1] = c01;
            kernel[0, 2] = c02;
            kernel[0, 3] = c03;
            kernel[0, 4] = c04;
            kernel[1, 0] = c10;
            kernel[1, 1] = c11;
            kernel[1, 2] = c12;
            kernel[1, 3] = c13;
            kernel[1, 4] = c14;
            kernel[2, 0] = c20;
            kernel[2, 1] = c21;
            kernel[2, 2] = c22;
            kernel[2, 3] = c23;
            kernel[2, 4] = c24;
            kernel[3, 0] = c30;
            kernel[3, 1] = c31;
            kernel[3, 2] = c32;
            kernel[3, 3] = c33;
            kernel[3, 4] = c34;
            kernel[4, 0] = c40;
            kernel[4, 1] = c41;
            kernel[4, 2] = c42;
            kernel[4, 3] = c43;
            kernel[4, 4] = c44;
            float[,] temp = new float[5, 5];
            // Check if the kernel is 1*1 3*3 or 5*5
            // Default always 1*1
            bool size2 = false;
            bool size3 = false;
            for (int i = 0; i < kernel.GetLength(0); i++)
            {
                for (int j = 0; j < kernel.GetLength(1); j++)
                {
                    if (kernel[i, j].Text == null || kernel[i, j].Text == "") temp[i, j] = 0;
                    else temp[i, j] = Convert.ToInt32(kernel[i, j].Text);
                }                
            }
            for (int i = 0; i < kernel.GetLength(0) && !size3 && !size2; i++)
            {
                if ((temp[i, 0] != 0 || temp[i,4] != 0) && !size3) size3 = true;
                if (!size2 && i > 0 && i < 4 && (temp[i, 1] != 0 || temp[i, 3] != 0)) size2 = true;
                for (int j = 0; j < kernel.GetLength(1) && !size3 && !size2; j++)
                {
                    if ((temp[0, j] != 0 || temp[4, j] != 0) && !size3) size3 = true;
                    if (!size2 && j > 0 && j < 4 && (temp[1, j] != 0 || temp[3, j] != 0)) size2 = true;
                }
            }
            // padding to read array
            int n = 0;
            float[,] convolution;
            if (size3) convolution = new float[5, 5];
            else if (size2)
            {
                n = 1;
                convolution = new float[3, 3];
            }
            else
            {
                convolution = new float[1, 1];
                convolution[0, 0] = (float)temp[2, 2];;
            }
            for (int i = n; i < 5 - n && (size2 || size3); i++)
            {
                for (int j = n; j < 5 - n && (size2 || size3); j++)
                {
                    convolution[i - n, j - n] = (float)temp[i, j];
                }
            }
            this.Close();
            window.Convolution(convolution);
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
