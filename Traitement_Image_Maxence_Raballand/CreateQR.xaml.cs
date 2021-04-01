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
    /// Logique d'interaction pour CreateQR.xaml
    /// </summary>
    public partial class CreateQR : Window
    {
        MainWindow window;

        public CreateQR(MainWindow window)
        {
            this.window = window;
            InitializeComponent();
        }

        private void Alphanumeric(object sender, TextCompositionEventArgs e)
        {
            char c = Convert.ToChar(e.Text);
            TextBox txtBox = (TextBox)sender;
            string supported = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ $%*+-./:abcdefghijklmnopqrstuvwxyz";
            if (supported.Contains(c) && txtBox.Text.Length < 47)
                e.Handled = false;
            else
                e.Handled = true;
            base.OnPreviewTextInput(e);
        }

        private void Validate(object sender, RoutedEventArgs e)
        {
            window.QRCodeClick(qrContent.Text, null);
            this.Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
