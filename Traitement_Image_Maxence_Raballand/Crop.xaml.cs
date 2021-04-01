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
    /// Logique d'interaction pour Crop.xaml
    /// </summary>
    public partial class Crop : Window
    {
        MainWindow window;
        int width, height, top, left, cropWidth, cropHeight;
        float ratio;

        public Crop(MainWindow window, int height, int width)
        {
            if (Math.Abs(height - 320) > Math.Abs(width - 440)) ratio = 320f / (float)height;
            else ratio = 440f / (float)width;
            this.window = window;
            InitializeComponent();
            this.width = width;
            this.height = height;
            imageBase.Height = cropper.Height = cropHeight = (int)(height * ratio);
            imageBase.Width = cropper.Width = cropWidth = (int)(width * ratio);
            UpdateSize();
            top = (int)cropper.Margin.Top;
            left = (int)cropper.Margin.Left;
        }

        /// <summary>
        /// updates visual boxxes for cropping
        /// </summary>
        private void UpdateSize()
        {
            height = (int)((float)cropHeight / ratio);
            width = (int)((float)cropWidth / ratio);
            size.Content = width + " X " + height;
            cropper.Height = cropHeight;
            cropper.Width = cropWidth;
            cropper.Margin = new Thickness(left, top, 0, 0);
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CropImage(object sender, RoutedEventArgs e)
        {
            int step = 5;
            if (sender == rightLeft && cropWidth - step > step)
            {
                cropWidth -= step;
                left -= step;
            }
            else if (sender == rightRight && left + cropWidth/2 + step <= imageBase.Width / 2)
            {
                cropWidth += step;
                left += step;
            }
            else if (sender == bottomBottom && top + cropHeight / 2 + step <= imageBase.Height / 2)
            {
                cropHeight += step;
                top += step;
            }
            else if (sender == bottomTop && height - step > step)
            {
                cropHeight -= step;
                top -= step;
            }
            else if (sender == leftLeft && - left + cropWidth / 2 + step <= imageBase.Width / 2)
            {
                left -= step;
                cropWidth += step;
            }
            else if (sender == leftRight && width - step > step)
            {
                cropWidth -= step;
                left += step;
            }
            else if (sender == topTop && - top + cropHeight / 2 + step <= imageBase.Height / 2)
            {
                top -= step;
                cropHeight += step;
            }
            else if (sender == topBottom && height - step > step)
            {
                top += step;
                cropHeight -= step;
            }
            UpdateSize();
        }

        /// <summary>
        /// Compute the new margins
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Validate(object sender, RoutedEventArgs e)
        {
            int left = (int)((float)this.left / ratio);
            int top = (int)((float)this.top / ratio);
            int dHeight = (int)(imageBase.Height / ratio - height);
            int dWidth = (int)(imageBase.Width / ratio - width);
            window.FreeCrop(dWidth / 2 + left / 2, dHeight / 2 + top / 2, dWidth / 2 - left / 2, dHeight / 2 - top / 2);
            this.Close();
        }
    }
}
