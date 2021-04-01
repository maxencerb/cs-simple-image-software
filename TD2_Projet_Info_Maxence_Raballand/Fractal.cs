using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TD2_Projet_Info_Maxence_Raballand
{
    /// <summary>
    /// Obtain Fractal from the instance MyImage
    /// </summary>
    public class Fractal : MyImage
    {

        /// <summary>
        /// tree non-finished
        /// </summary>
        /// <param name="type">fractal type mandelbrot</param>
        /// <param name="args">
        /// mandelbrot : 0:zoom (default:1)
        /// </param>
        private Fractal(string type, string[] args = null)
        {
             // work with args
            offsetSize = 54;
            imageType = "BM";
            int zoom = 1;
            switch (type)
            {

                case "mandelbrot":
                    if (args != null)
                    {
                        if (args.Length > 0) int.TryParse(args[0], out zoom);
                    }
                    MandelbrotFractal(zoom);
                    break;
                case "julia":
                    if (args != null)
                    {
                        if (args.Length > 0) int.TryParse(args[0], out zoom);
                    }
                    JuliaFractal(zoom);
                    break;
            }
        } 

        /// <summary>
        /// Creates a mandelbrot fractal
        /// </summary>
        /// <returns>the image of a mandelbrot fractal</returns>
        public static Fractal Mandelbrot()
        {
            return new Fractal("mandelbrot");
        }

        /// <summary>
        /// Creates a julia Fractal
        /// </summary>
        /// <returns>image of a julia fractal</returns>
        public static Fractal Julia()
        {
            return new Fractal("julia");
        }

        /// <summary>
        /// Uses an algorithm to create the mandelbrot fractal
        /// </summary>
        /// <param name="zoom">the amount of zoom in the mandelbrot fractal (default 1)</param>
        private void MandelbrotFractal(int zoom = 1)
        {
            float x1 = -2.1f;
            float x2 = 0.6f;
            float y1 = -1.2f;
            float y2 = 1.2f;
            float zoomf = zoom * 100;
            int max = 255;
            image = new Pixel[(int)((y2 - y1) * zoomf), (int)((x2 - x1) * zoomf)];
            for (int y = 0; y < image.GetLength(0); y++)
            {
                for (int x = 0; x < image.GetLength(1); x++)
                {
                    float c_r = (float)x / zoomf + x1;
                    float c_i = (float)y / zoomf + y1;
                    float z_r = 0;
                    float z_i = 0;
                    int i = 0;
                    do
                    {
                        float temp = z_r;
                        z_r = z_r * z_r - z_i * z_i + c_r;
                        z_i = 2 * z_i * temp + c_i;
                        i++;
                    } while (z_r * z_r + z_i * z_i < 4 && i < max);
                    if (i == max) image[y, x] = Pixel.Black;
                    else image[y, x] = new Pixel(0, 0, (byte)(i * 255 / max));
                }
            }
        }

        /// <summary>
        /// Algorithm to create the Julia Fractal
        /// </summary>
        /// <param name="zoom">zoom default 1</param>
        private void JuliaFractal(int zoom = 1)
        {
            float x1 = -1f;
            float x2 = 1f;
            float y1 = -1.2f;
            float y2 = 1.2f;
            float zoomf = zoom * 100;
            int max = 255;
            image = new Pixel[(int)((y2 - y1) * zoomf), (int)((x2 - x1) * zoomf)];
            for (int y = 0; y < image.GetLength(0); y++)
            {
                for (int x = 0; x < image.GetLength(1); x++)
                {
                    float c_r = 0.285f;
                    float c_i = 0.01f;
                    float z_r = x / zoomf + x1;
                    float z_i = y / zoomf + y1;
                    int i = 0;
                    do
                    {
                        float temp = z_r;
                        z_r = z_r * z_r - z_i * z_i + c_r;
                        z_i = 2 * z_i * temp + c_i;
                        i++;
                    } while (z_r * z_r + z_i * z_i < 4 && i < max);
                    if (i == max) image[y, x] = Pixel.Black;
                    else image[y, x] = new Pixel(0, 0, (byte)(i * 255 / max));
                }
            }
        }
    }
}
