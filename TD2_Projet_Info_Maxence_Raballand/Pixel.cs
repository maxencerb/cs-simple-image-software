using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TD2_Projet_Info_Maxence_Raballand
{
    /// <summary>
    /// Pixel with RGB colors only and Values Between 0 and 255
    /// </summary>
    public class Pixel
    {
        /// <summary>
        /// red byte
        /// </summary>
        public byte R { get; set; }

        /// <summary>
        /// Green byte
        /// </summary>
        public byte G { get; set; }

        /// <summary>
        /// Blue byte
        /// </summary>
        public byte B { get; set; }

        /// <summary>
        /// For Grey Shades
        /// </summary>
        /// <param name="greyShade">from black (0) to white (255)</param>
        public Pixel(byte greyShade)
        {
            R = G = B = greyShade;
        }

        /// <summary>
        /// For RGB
        /// </summary>
        /// <param name="R"></param>
        /// <param name="G"></param>
        /// <param name="B"></param>
        public Pixel(byte R, byte G, byte B)
        {
            this.R = R;
            this.G = G;
            this.B = B;
        }

        /// <summary>
        /// For pixel
        /// </summary>
        /// <param name="pixel"></param>
        public Pixel(Pixel pixel)
        {
            R = pixel.R;
            G = pixel.G;
            B = pixel.B;
        }

        /// <summary>
        /// Turn Color into its corresponding grey shade
        /// </summary>
        public void ToGreyShade()
        {
            int mid = (R + G + B) / 3;
            R = G = B = (byte)mid;
        }

        /// <summary>
        /// Return a string that describes its RGB values
        /// </summary>
        /// <returns>RGB Values</returns>
        public override string ToString()
        {
            return "Red : " + R + " Green : " + G + " Blue : " + B;
        }

        /// <summary>
        /// Set value of a pixel to another pixel without changing its object reference
        /// </summary>
        /// <param name="pixel">pixel to set the value to</param>
        public void SetValueTo(Pixel pixel)
        {
            R = pixel.R;
            G = pixel.G;
            B = pixel.B;
        }
        
        /// <summary>
        /// Creates a new pixel which is the average color of four pixels
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <returns>the average pixel</returns>
        public static Pixel Average(Pixel p1, Pixel p2, Pixel p3, Pixel p4)
        {
            int R = (p1.R + p2.R + p3.R + p4.R) / 4;
            int G = (p1.G + p2.G + p3.G + p4.G) / 4;
            int B = (p1.B + p2.B + p3.B + p4.B) / 4;
            return new Pixel((byte)R, (byte)G, (byte)B);
        }

        /// <summary>
        /// New pixel average of 2 pixels
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Pixel Average(Pixel p1, Pixel p2)
        {
            return Average(p1, p2, p1, p2);
        }

        /// <summary>
        /// convert a pixel to either black or white
        /// </summary>
        public void ToBlackAndWhite()
        {
            ToGreyShade();
            if ((float)R < 127.5f) R = G = B = 0;
            else R = G = B = 255;
        }

        /// <summary>
        /// returns the pixel value multiplied by a float (each volor value is multiplicated)
        /// </summary>
        /// <param name="a">the value to multiply by</param>
        /// <param name="b">the pixel being multiblied</param>
        /// <returns>A pixel multiplied by a float</returns>
        public static Pixel operator *(float a, Pixel b)
        {
            return new Pixel((byte)(a * b.R), (byte)(a * b.G), (byte)(a * b.B));
        }

        /// <summary>
        /// Adds up two pixels together
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>pixel corresponding to the sum</returns>
        public static Pixel operator +(Pixel a, Pixel b)
        {
            return new Pixel((byte)(a.R + b.R), (byte)(a.G + b.G), (byte)(a.B + b.B));
        }

        /// <summary>
        /// Inverse the color value of the pixel
        /// </summary>
        public void Inverse()
        {
            R = (byte)(255 - R);
            G = (byte)(255 - G);
            B = (byte)(255 - B);
        }

        //--------------------------Basic colours-------------------------------//

        /// <summary>
        /// Pixel to black
        /// </summary>
        public void ToBlack()
        {
            R = G = B = 0;
        }

        /// <summary>
        /// Creates a black pixel
        /// </summary>
        /// <returns>black pixel</returns>
        public static Pixel Black
        {
            get
            {
                return new Pixel(0);
            }
        }

        /// <summary>
        /// Pixel to white
        /// </summary>
        public void ToWhite()
        {
            R = G = B = 255;
        }

        /// <summary>
        /// Creates a white pixel
        /// </summary>
        /// <returns>white pixel</returns>
        public static Pixel White
        {
            get
            {
                return new Pixel(255);
            }
        }

        /// <summary>
        /// Make the value of red maximum
        /// </summary>
        public void AddRed()
        {
            R = 255;
        }

        /// <summary>
        /// Make the value of green maximum
        /// </summary>
        public void AddGreen()
        {
            G = 255;
        }

        /// <summary>
        /// Make the value of blue maximum
        /// </summary>
        public void AddBlue()
        {
            B = 255;
        }

        /// <summary>
        /// Define if a pixel is close to the color black
        /// </summary>
        public bool isCloseToBlack
        {
            get
            {
                int maximum = 30; //arbitrary value
                return R < maximum && G < maximum && B < maximum;
            }
        }

        /// <summary>
        /// Defines if a pixel is close to the color white
        /// </summary>
        public bool isCloseToWhite
        {
            get
            {
                int minimum = 225; //arbitrary value
                return R > minimum && G > minimum && B > minimum;
            }
        }

        /// <summary>
        /// Defines an operator for the division between two pixels
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>e new pixel which is the result of the division</returns>
        public static Pixel operator /(Pixel a, Pixel b)
        {
            byte R = 0, G = 0, B = 0;
            if (b.R == 0) R = a.R;
            else R = (byte)(a.R / b.R);
            if (b.G == 0) R = a.G;
            else R = (byte)(a.G / b.G);
            if (b.B == 0) R = a.B;
            else B = (byte)(a.B / b.B);
            return new Pixel(R, G, B);
        }

        /* make only one color stand out */
        
        /// <summary>
        /// Says if colors are about the same
        /// </summary>
        /// <param name="a">color A</param>
        /// <param name="b">color B</param>
        /// <returns>true if they are about the same</returns>
        public static bool ApproxEquals(int a, int b)
        {
            return Math.Abs(a - b) < 4;
        }

        public void RedFilter()
        {
            if (!((float)R/(float)G > 1.9f && (float)R / (float)B > 1.9f)) 
            {
                /*G /= 2;
                B /= 2;*/
                ToGreyShade();
            }
        }
    }
}
