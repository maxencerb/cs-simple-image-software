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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using TD2_Projet_Info_Maxence_Raballand;
using System.IO;
using System.ComponentModel;

namespace Traitement_Image_Maxence_Raballand
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Stack<string> selectedFileName;
        Stack<string> histogramSource;
        MyImage img;
        OpenFileDialog dlg;
        SaveFileDialog sDlg;
        bool started;

        public MainWindow(string source)
        {
            InitializeComponent();
            Histogramme.Visibility = Visibility.Hidden;
            dlg = new OpenFileDialog();
            dlg.Filter = "Image files (*.bmp)|*.bmp";
            selectedFileName = new Stack<string>();
            histogramSource = new Stack<string>();
            if (source != "")
            {
                selectedFileName.Push(source);
                InitButtons();
                SetImageSource(source);
            }
            sDlg = new SaveFileDialog();
            sDlg.Filter = "Image files (*.bmp)|*.bmp";
            started = false;
            System.Windows.Application.Current.MainWindow.Closing += new CancelEventHandler(OnWindowClosing);
        }

        public MainWindow()
        {
           InitializeComponent();
            Histogramme.Visibility = Visibility.Hidden;
            dlg = new OpenFileDialog();
            dlg.Filter = "Image files (*.bmp)|*.bmp";
            selectedFileName = new Stack<string>();
            histogramSource = new Stack<string>();
            sDlg = new SaveFileDialog();
            sDlg.Filter = "Image files (*.bmp)|*.bmp";
            started = false;
            System.Windows.Application.Current.MainWindow.Closing += new CancelEventHandler(OnWindowClosing);
        }

        /* https://stackoverflow.com/questions/3683450/handling-the-window-closing-event-with-wpf-mvvm-light-toolkit */
        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (img != null)
            {
                // https://www.c-sharpcorner.com/UploadFile/mahesh/messagebox-in-wpf/
                MessageBoxResult result = System.Windows.MessageBox.Show("Voulez vous sauvegardez le travail en cours ?", "Sauvgardez votre travail", MessageBoxButton.YesNo);
                if(result == MessageBoxResult.Yes)
                {
                    ImageViewer.Source = null;
                    while (selectedFileName.Count > 1) File.Delete(selectedFileName.Pop());
                    while (histogramSource.Count > 0) File.Delete(selectedFileName.Pop());
                    img.Save(selectedFileName.Peek());
                }
                else if (result == MessageBoxResult.No)
                {
                    ImageViewer.Source = null;
                    while (selectedFileName.Count > 1) File.Delete(selectedFileName.Pop());
                    while (histogramSource.Count > 0) File.Delete(histogramSource.Pop());
                }
            }
        }

        /// <summary>
        /// Browse a bmp file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Browse(object sender, RoutedEventArgs e)
        {
            if (img != null) OnWindowClosing(null, null);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                selectedFileName.Clear();
                histogramSource.Clear();
                selectedFileName.Push(dlg.FileName);
                SetImageSource(selectedFileName.Peek());
                img = new MyImage(selectedFileName.Peek());
                if (selectedFileName.Peek() != "" && !started) InitButtons();
            }
        }

        /// <summary>
        /// Init the placement of the buttons
        /// </summary>
        private void InitButtons()
        {
            FetchFileButton.Content = "CHANGER D'IMAGE...";
            FetchFileButton.VerticalAlignment = VerticalAlignment.Top;
            FetchFileButton.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            FetchFileButton.Margin = new Thickness(0, 30, 10, 0);
            FetchFileButton.Width = 130;
            Histogramme.Visibility = Visibility.Visible;
            started = true;
        }

        /// <summary>
        /// Set new source for image viewer
        /// </summary>
        /// <param name="filename"></param>
        private void SetImageSource(string filename)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(filename);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            ImageViewer.Source = bitmap;
        }

        private void Histogram(Object sender, RoutedEventArgs e)
        {
            while (histogramSource.Count < selectedFileName.Count)
                histogramSource.Push(img.Histogram(selectedFileName.Peek()));
            Window histogram = new HistogramWindow(histogramSource.Peek());
            histogram.Show();
        }

        private void BlackAndWhite(object sender, RoutedEventArgs e)
        {
            if (selectedFileName.Count > 0)
            {
                img.ToBlackAndWhite();
                selectedFileName.Push(img.Save(selectedFileName.First()));
                SetImageSource(selectedFileName.Peek());
            }
        }

        /// <summary>
        /// Modify image functions
        /// </summary>
        /// <param name="function">function name (Rotate/ReduceSize/Zoom)</param>
        /// <param name="value"></param>
        public void Modify(string function, float value)
        {
            switch (function)
            {
                case "Rotate":
                    img.Rotate(value);
                    break;

                case "ReduceSize":
                    img.ReduceSize((int)value);
                    break;

                case "Zoom":
                    img.Zoom(value);
                    break;
            }
            selectedFileName.Push(img.Save(selectedFileName.First()));
            SetImageSource(selectedFileName.Peek());
        }

        private void Rotate(object sender, RoutedEventArgs e)
        {
            if (img != null)
            {
                Window window = new SettingsWindow("Choisir le degré de rotation (en °)", -179, 180, this, "Rotate", "Rotate - Maxence Raballand");
                window.ShowDialog();
            }
        }

        private void Undo(object sender, RoutedEventArgs e)
        {
            if (selectedFileName.Count > 1)
            {
                ImageViewer.Source = null;
                File.Delete(selectedFileName.Pop());
                SetImageSource(selectedFileName.Peek());
                img = new MyImage(selectedFileName.Peek());
            }
            if (histogramSource.Count > 1) File.Delete(histogramSource.Pop());
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (selectedFileName.Count > 0)
            {
                while (selectedFileName.Count > 1) File.Delete(selectedFileName.Pop());
                selectedFileName.Push(img.Save(selectedFileName.Peek()));
                SetImageSource(selectedFileName.Peek());
                histogramSource.Clear();
            }
        }

        private void SaveAs(object sender, RoutedEventArgs e)
        {
            if (selectedFileName.Count > 0)
            {
                sDlg.InitialDirectory = selectedFileName.First();
                if (sDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    while (selectedFileName.Count > 1) File.Delete(selectedFileName.Pop());
                    img.Save(sDlg.FileName);
                    selectedFileName.Clear();
                    selectedFileName.Push(sDlg.FileName);
                    SetImageSource(selectedFileName.Peek());
                }
            }
        }

        private void Mirror(object sender, RoutedEventArgs e)
        {
            if (img != null)
            {
                img.mirror();
                selectedFileName.Push(img.Save(selectedFileName.First()));
                SetImageSource(selectedFileName.Peek());
            }
        }

        private void Square(object sender, RoutedEventArgs e)
        {
            if (img != null)
            {
                img.square();
                selectedFileName.Push(img.Save(selectedFileName.First()));
                SetImageSource(selectedFileName.Peek());
            }
        }

        private void GreyShades(object sender, RoutedEventArgs e)
        {
            if (img != null)
            {
                img.ToGreyShades();
                selectedFileName.Push(img.Save(selectedFileName.First()));
                SetImageSource(selectedFileName.Peek());
            }
        }

        private void ReduceSize(object sender, RoutedEventArgs e)
        {
            if (img != null)
            {
                Window window = new SettingsWindow("Choisir le coefficient de rétrécissement", 2, 10, this, "ReduceSize", "Reduce Size - Maxence Raballand");
                window.ShowDialog();
            }
        }

        private void Zoom(object sender, RoutedEventArgs e)
        {
            if (img != null)
            {
                Window window = new SettingsWindow("Choisir le coefficient de zoom", 1, 10, this, "Zoom", "Zoom - Maxence Raballand");
                window.ShowDialog();
            }
        }

        private void CropFromMiddle(object sender, RoutedEventArgs e)
        {

        }

        private void IncreaseSize(object sender, RoutedEventArgs e)
        {
            if (img != null)
            {
                img.IncreaseSize();
                selectedFileName.Push(img.Save(selectedFileName.First()));
                SetImageSource(selectedFileName.Peek());
            }
        }

        private void Inverse(object sender, RoutedEventArgs e)
        {
            if (img != null)
            {
                img.Inverse();
                selectedFileName.Push(img.Save(selectedFileName.First()));
                SetImageSource(selectedFileName.Peek());
            }
        }

        private void Blur(object sender, RoutedEventArgs e)
        {
            Convolution(MyImage.Blur);
        }

        private void GaussianBlur(object sender, RoutedEventArgs e)
        {
            Convolution(MyImage.GaussianBlur5);
        }

        private void EdgeDetection(object sender, RoutedEventArgs e)
        {
            float[,] convo = null;
            if (sender == horizontalEdge) convo = MyImage.HorizontalEdgeDetection;
            else if (sender == verticalEdge) convo = MyImage.VerticalEdgeDetection;
            else if (sender == autoedge) convo = MyImage.EdgeDetection;
            if (convo != null) Convolution(convo);
        }

        private void ConvolutionPerso(object sender, RoutedEventArgs e)
        {
            Window convo = new Convolution(this);
            convo.ShowDialog();
        }

        private void Fractales(object sender, RoutedEventArgs e)
        {
            if (img != null) OnWindowClosing(null, null);
            if (sender == mandelbrot) img = Fractal.Mandelbrot();
            else if (sender == julia) img = Fractal.Julia();
            if (sDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                img.Save(sDlg.FileName);
                if (!started) InitButtons();
                SetImageSource(sDlg.FileName);
            }
        }

        private void Decrypt(object sender, RoutedEventArgs e)
        {
            if (img != null)
            {
                MyImage imgMain = img.Decrypt();
                selectedFileName.Push(img.Save(selectedFileName.First()));
                SetImageSource(selectedFileName.Peek());
                Window windowMainImage = new MainWindow(imgMain.Save(selectedFileName.First()));
                windowMainImage.ShowDialog();
            }
        }

        public void QRCodeClick(object sender, RoutedEventArgs e)
        {
            if(sender == createQR)
            {
                //Creation of the QRCode
                OnWindowClosing(null, null);
                Window window = new CreateQR(this);
                window.ShowDialog();
            }
            else if (sender == decryptQR && img != null)
            {
                string result = "Impossible de décoder ce QRCode\nL'image doit être clair et le type de QRCode 1 ou 2 avec masque 0 et alphanumérique uniquement.\nVeillez à avoir le code droit sur un fond blanc.";
                try
                {
                    result = QRCode.Decode(img);
                    System.Windows.Clipboard.SetText(result);
                    result = "Le Code Signifie : " + result + "\nLe texte a été collé dans le presse papier";
                }
                catch(Exception ex)
                {

                }
                finally
                {
                    System.Windows.MessageBox.Show(result, "QRCode Decoder", MessageBoxButton.OK);
                }
            }
            else if(sender.GetType() == typeof(string))
            {
                string qrContent = (string)sender;
                img = QRCode.Encode(qrContent);
                sDlg.Title = "Sauvegardez votre QR Code";
                if (sDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    selectedFileName.Push(img.Save(sDlg.FileName));
                    InitButtons();
                    SetImageSource(sDlg.FileName);
                }
                sDlg.Title = "Save image";
            }
        }

        private void Encrypt(object sender, RoutedEventArgs e)
        {
            if (img != null)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Vous devez choisir une image pour cacher l'image actuelle\nPS : pour un meilleur résultat prenez une image avec un rapport hauteur/largeur similaire et en couleurs.", "Encryption d'une image", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    if(dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        MyImage encrypted = new MyImage(dlg.FileName);
                        img.Encrypt(encrypted);
                        selectedFileName.Push(img.Save(selectedFileName.First()));
                        SetImageSource(selectedFileName.Peek());
                    }
                }
            }
        }

        private void FreeCrop(object sender, RoutedEventArgs e)
        {
            if (img != null)
            {
                Window window = new Crop(this, img.image.GetLength(0), img.image.GetLength(1));
                window.ShowDialog();
            }
        }

        /// <summary>
        /// FreeCrop methode for other windows
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="rigth"></param>
        /// <param name="bottom"></param>
        public void FreeCrop(int left, int top, int rigth, int bottom)
        {
            img.CropFromLTRB(left, top, rigth, bottom);
            selectedFileName.Push(img.Save(selectedFileName.First()));
            SetImageSource(selectedFileName.Peek());
        }

        /// <summary>
        /// General convolution method
        /// </summary>
        /// <param name="kernel"></param>
        public void Convolution(float[,] kernel)
        {
            if (img != null)
            {
                string temp = Title;
                Title += " - Loading...";
                img.Convolution(kernel);
                selectedFileName.Push(img.Save(selectedFileName.First()));
                SetImageSource(selectedFileName.Peek());
                Title = temp;
            }
        }
    }
}
