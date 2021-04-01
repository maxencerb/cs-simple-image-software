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
    /// Logique d'interaction pour SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        string function;
        MainWindow window;

        public SettingsWindow(string label, float minimum, float maximum, MainWindow window, string function, string title = "Settings window - Maxence Raballand")
        {
            InitializeComponent();
            Title = title;
            this.label.Content = label;
            slider.Minimum = minimum;
            slider.Maximum = maximum;
            this.window = window;
            this.function = function;
        }

        private void Validate(object sender, RoutedEventArgs e)
        {
            window.Modify(function, (float)slider.Value);
            this.Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
