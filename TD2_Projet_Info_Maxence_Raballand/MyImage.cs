using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using System.IO;

namespace TD2_Projet_Info_Maxence_Raballand
{
    /// <summary>
    /// Creates an image of pixel to work on
    /// Able to communicate with '.bmp' type file
    /// </summary>
    public class MyImage
    {
        /// <summary>
        /// Type of image (only "BM" is supported
        /// </summary>
        public string imageType { get; protected set; }

        /// <summary>
        /// OffSet to start reading data
        /// </summary>
        public int offsetSize { get; protected set; }

        /// <summary>
        /// Pixel array containing the image
        /// </summary>
        public Pixel[,] image { get; protected set; }

        /// <summary>
        /// Necessary for the creation of heritage
        /// </summary>
        protected MyImage()
        {
            offsetSize = 54;
            imageType = "BM";
        }

        /// <summary>
        /// image builder for existing image
        /// </summary>
        /// <param name="filename"></param>
        public MyImage(string filename)
        {
            if(File.Exists(filename)) FromFileToImage(filename);
            else
            {
                offsetSize = 54;
                imageType = "BM";
                image = null;
            }
        }

        /// <summary>
        /// Creates an instance of the class MyImage with an array of Pixel
        /// </summary>
        /// <param name="img"></param>
        public MyImage(Pixel[,] img, bool copy = true)
        {
            imageType = "BM";
            offsetSize = 54;
            if (copy) image = img;
            else
            {
                image = new Pixel[img.GetLength(0), img.GetLength(1)];
                for (int i = 0; i < img.GetLength(0); i++)
                    for (int j = 0; j < img.GetLength(1); j++)
                        image[i, j] = new Pixel(img[i, j]);
            }
        }

        /// <summary>
        /// Creates an histogram based on the image
        /// </summary>
        /// <param name="filename">specify the folder and filename</param>
        /// <returns>
        /// returns a string with the filename 
        /// itcan vary because of existing file in the folder
        /// </returns>
        public string Histogram(string filename)
        {
            imageType = "BM";
            offsetSize = 54;
            int[,] histogram = this.histogram;
            int max = 0;
            foreach (int n in histogram)
                if (n > max)
                    max = n;
            float ratio = 100f / (float)max;
            Pixel[,] imageTemp = new Pixel[100, 256];
            for (int i = 0; i < imageTemp.GetLength(1); i++)
            {
                for (int j = 0; j < imageTemp.GetLength(0); j++)
                {
                    imageTemp[imageTemp.GetLength(0) - j - 1, imageTemp.GetLength(1) - i - 1] = Pixel.Black;
                    if (histogram[0, i] * ratio >= j) imageTemp[imageTemp.GetLength(0) - j - 1, imageTemp.GetLength(1) - i - 1].AddRed();
                    if (histogram[1, i] * ratio >= j) imageTemp[imageTemp.GetLength(0) - j - 1, imageTemp.GetLength(1) - i - 1].AddGreen();
                    if (histogram[2, i] * ratio >= j) imageTemp[imageTemp.GetLength(0) - j - 1, imageTemp.GetLength(1) - i - 1].AddBlue();
                }
            }
            MyImage histo = new MyImage(imageTemp);
            string[] split = filename.Split('\\');
            filename = "";
            for (int i = 0; i < split.Length-1; i++)
            {
                filename += split[i] + '\\';
            }
            filename += "histogram-" + split.Last();
            return histo.Save(filename);
        }

        /// <summary>
        /// Creates an array 3*256 representing the occurence of each values for R, G and B
        /// </summary>
        public int[,] histogram
        {
            get
            {
                int[,] histogram = new int[3, 256];
                for (int i = 0; i < histogram.GetLength(0); i++)
                    for (int j = 0; j < histogram.GetLength(1); j++)
                        histogram[i, j] = 0;
                foreach(Pixel pixel in image)
                {
                    histogram[0, pixel.R]++;
                    histogram[1, pixel.G]++;
                    histogram[2, pixel.B]++;
                }
                return histogram;
            }
        }

        /// <summary>
        /// Save the instance of MyImage class into a bmp 24bit image
        /// </summary>
        /// <param name="filename">path to image</param>
        /// <returns>
        /// the path to the file
        /// Can Vary due to existing files in folder
        /// </returns>
        public string Save(string filename)
        {
            if (File.Exists(filename))
            {
                //Find a correct filename
                string[] split = filename.Split('.');
                filename = "";
                for (int i = 0; i < split.Length - 1; i++)
                {
                    filename += split[i];
                    if (i < split.Length - 2) filename += '.';
                }
                int n = 1;
                while (File.Exists(filename + '(' + n + ").bmp"))
                    n++;
                filename = filename + '(' + n + ").bmp";
            }
            FromImageToFile(filename);
            return filename;
        }

        /// <summary>
        /// Convert a file into an instance of the class MyImage
        /// </summary>
        /// <param name="filename">hte path to file</param>
        private void FromFileToImage(string filename)
        {
            byte[] file = File.ReadAllBytes(filename);
            imageType = Convert.ToChar(file[0]).ToString() + Convert.ToChar(file[1]).ToString();
            int fileSize = EndianToInt(SubArray(file, 2, 4));
            offsetSize = EndianToInt(SubArray(file, 10, 4));
            int width = EndianToInt(SubArray(file, 18, 4));
            int height = EndianToInt(SubArray(file, 22, 4));
            image = new Pixel[height, width];
            int padding = (4 - (width * 3) % 4) % 4;
            switch (imageType)
            {
                //for bitmap images
                case "BM":
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            //position in file
                            int pos = offsetSize + (i * width + j) * 3 + i * padding;
                            image[height - i - 1, j] = new Pixel(file[pos + 2], file[pos + 1], file[pos]);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Convert an instance of the class MyImage into a 24bit bmp Image
        /// </summary>
        /// <param name="filename">path to file</param>
        private void FromImageToFile(string filename)
        {
            switch (imageType)
            {
                case "BM":
                    int padding = (4 - (image.GetLength(1) * 3) % 4) % 4;
                    byte[] file = new byte[image.Length * 3 + offsetSize + padding * image.GetLength(0)];
                    file[0] = 66;
                    file[1] = 77;//file type 2 bytes
                    Insert(file, IntToEndian(file.Length), 2); // file size 4 bytes
                    Insert(file, IntToEndian(0), 6); //infos for softwares set to 0 2*2 bytes
                    Insert(file, IntToEndian(offsetSize), 10); //offsetsize 4 bytes
                    Insert(file, IntToEndian(40), 14); //header size 4 bytes
                    Insert(file, IntToEndian(image.GetLength(1)), 18); //width 4 bytes
                    Insert(file, IntToEndian(image.GetLength(0)), 22); //heigth 4 bytes
                    Insert(file, IntToEndian(1), 26); //color planes 2 bytes
                    Insert(file, IntToEndian(24), 28); //bits per pixel 2 bytes
                    Insert(file, IntToEndian(0), 30); //compression 4 bytes
                    Insert(file, IntToEndian(0), 34); //Imagesize 4 bytes
                    Insert(file, IntToEndian(0), 38); //Xppm 4 bytes
                    Insert(file, IntToEndian(0), 42); //Yppm 4 bytes
                    Insert(file, IntToEndian(0), 46); //total colors 4 bytes
                    Insert(file, IntToEndian(0), 50); //important colors 4 bytes
                    for (int i = 0; i < image.GetLength(0); i++)                                                                                                                                                                                                          
                    {
                        for (int j = 0; j < image.GetLength(1) + padding; j++)
                        {
                            int pos = offsetSize + (i * image.GetLength(1) + j) * 3 + i * padding;
                            if (j >= image.GetLength(1))
                            {
                                file[pos] = 0;
                                if (padding > 1)
                                {
                                    file[pos + 1] = 0;
                                    j++;
                                }
                                if (padding > 2)
                                {
                                    file[pos + 2] = 0;
                                    j++;
                                }
                            }
                            else
                            {
                                file[pos] = image[image.GetLength(0) - i - 1, j].B;
                                file[pos + 1] = image[image.GetLength(0) - i - 1, j].G;
                                file[pos + 2] = image[image.GetLength(0) - i - 1, j].R;
                            }
                        }
                    }
                    File.WriteAllBytes(filename, file);
                    break;
            }
        }

        /// <summary>
        /// Convert from little endian to int
        /// </summary>
        /// <param name="bytes">byte array in little endian</param>
        /// <returns>the int that represents the byte array</returns>
        public static int EndianToInt(byte[] bytes)
        {
            int result = 0;
            for (int i = 0; i < bytes.Length; i++)
                result += bytes[i] * (int)Math.Pow(256, i);
            return result;
        }

        /// <summary>
        /// Convert int to endian
        /// </summary>
        /// <param name="n">number to convert</param>
        /// <returns>the array of byte in LE (width of 4)</returns>
        public static byte[] IntToEndian(int n)
        {
            byte[] bytes = BitConverter.GetBytes(n);
            if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }

        //method to create a subarray (inspired from stackoverflow for the T[])
        /// <summary>
        /// extract a subarray from an existing array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">array to extract from</param>
        /// <param name="start">start index</param>
        /// <param name="length">length of the new array</param>
        /// <returns>an array contained in the array</returns>
        public static T[] SubArray<T>(T[] array, int start, int length)
        {
            T[] subArray = new T[length];
            if (length + start > array.Length) throw new Exception("paramètres incorrect");
            for (int i = start; i < length + start; i++)
            {
                subArray[i - start] = array[i];
            }
            return subArray;
        }

        /// <summary>
        /// insert array value in a second array
        /// </summary>
        /// <param name="array1">array for insertion</param>
        /// <param name="array2">array to insert</param>
        /// <param name="start">start position for insert</param>
        private static void Insert(byte[] array1, byte[] array2, int start)
        {
            for (int i = 0; i < array2.Length; i++)
                array1[i + start] = array2[i];
        }

        /// <summary>
        /// Clean the values with 0
        /// </summary>
        /// <param name="array">array to clean</param>
        /// <param name="start">start position</param>
        /// <param name="length">length to clean</param>
        private static void Clean(byte[] array, int start, int length)
        {
            for (int i = 0; i < length; i++)
                array[start + i] = 0;
        }

        //méthodes sur image

        /// <summary>
        /// image to grey shade
        /// </summary>
        public void ToGreyShades()
        {
            foreach (Pixel pix in image)
                pix.ToGreyShade();
        }

        /// <summary>
        /// image to black and white
        /// </summary>
        public void ToBlackAndWhite()
        {
            foreach (Pixel pix in image)
                pix.ToBlackAndWhite();
        }

        /// <summary>
        /// Mirror an image
        /// </summary>
        /// <param name="right">if true keeps the right part else the left part</param>
        public void mirror(bool right = true)
        {
           for (int i = 0; i < image.GetLength(0); i++)
           {
                for (int j = 0; j < image.GetLength(1)/2; j++)
                {
                    if (right) image[i, j].SetValueTo(image[i, image.GetLength(1) - j -1]);
                    else image[i, image.GetLength(1) - j - 1].SetValueTo(image[i, j]);
                }
           }
           
        }

        /// <summary>
        /// Create a square with an image
        /// </summary>
        public void square()
        {
            if (image.GetLength(1) < image.GetLength(0))
            {
                Pixel[,] imageTemp = new Pixel[image.GetLength(1), image.GetLength(1)];
                for (int i = 0; i < image.GetLength(1); i++)
                {
                    for (int j = 0; j < image.GetLength(1); j++)
                    {
                        imageTemp[i, j] = image[(image.GetLength(0) - image.GetLength(1)) / 2 + i, j];
                    }
                }
                image = imageTemp;
            }
            else
            {
                Pixel[,] imageTemp = new Pixel[image.GetLength(0), image.GetLength(0)];
                for (int i = 0; i < image.GetLength(0); i++)
                {
                    for (int j = 0; j < image.GetLength(0); j++)
                    {
                        imageTemp[i, j] = image[i, (image.GetLength(1) - image.GetLength(0)) / 2 + j];
                    }
                }
                image = imageTemp;
            }
        }

        /// <summary>
        /// shortens an image by a 2 ratio
        /// </summary>
        public void ReduceSize(int ratio)
        {
            Crop(image.GetLength(0) - image.GetLength(0) % ratio, image.GetLength(1) - image.GetLength(1) % ratio);
            Pixel[,] imageTemp = new Pixel[image.GetLength(0) / ratio, image.GetLength(1) / ratio];
            for (int i = 0; i < imageTemp.GetLength(0); i++)
            {
                for (int j = 0; j < imageTemp.GetLength(1); j++)
                {
                    int R = 0, G = 0, B = 0;
                    for (int k = 0; k < ratio; k++)
                    {
                        for (int l = 0; l < ratio; l++)
                        {
                            R += image[i * ratio + k, j * ratio + l].R;
                            G += image[i * ratio + k, j * ratio + l].G;
                            B += image[i * ratio + k, j * ratio + l].B;
                        }
                    }
                    Pixel pixel = new Pixel((byte)(R / (ratio * ratio)), (byte)(G / (ratio * ratio)), (byte)(B / (ratio * ratio)));
                    imageTemp[i, j] = pixel;
                    //Console.WriteLine(i / ratio + " " + j / ratio + " " + imageTemp.GetLength(0) + " " + imageTemp.GetLength(1));
                }
            }
            image = imageTemp;
        }

        /// <summary>
        /// Increase the size of the image using interpolation
        /// </summary>
        public void IncreaseSize()
        {
            int height = image.GetLength(0) * 2;
            int width = image.GetLength(1) * 2;
            Pixel[,] imageTemp = new Pixel[height, width];
            for (int i = 0; i < image.GetLength(0); i++)
                for (int j = 0; j < image.GetLength(1); j++)
                    imageTemp[i * 2, j * 2] = image[i, j];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (imageTemp[i, j] == null)
                    {
                        if (i % 2 == 0 && j == width - 1) imageTemp[i, j] = new Pixel(imageTemp[i, j - 1]);
                        else if (j % 2 == 0 && i == height - 1) imageTemp[i, j] = new Pixel(imageTemp[i - 1, j]);
                        else if (j == width - 1 && i == height - 1) imageTemp[i, j] = new Pixel(imageTemp[i - 1, j - 1]);
                        else if (j == width - 1) imageTemp[i, j] = Pixel.Average(imageTemp[i - 1, j - 1], imageTemp[i + 1, j - 1]);
                        else if (i == height - 1) imageTemp[i, j] = Pixel.Average(imageTemp[i - 1, j - 1], imageTemp[i - 1, j + 1]);
                        else if (i % 2 == 0) imageTemp[i, j] = Pixel.Average(imageTemp[i, j - 1], imageTemp[i, j + 1]);
                        else if (j % 2 == 0) imageTemp[i, j] = Pixel.Average(imageTemp[i - 1, j], imageTemp[i + 1, j]);
                        else imageTemp[i, j] = Pixel.Average(imageTemp[i - 1, j - 1], imageTemp[i - 1, j + 1], imageTemp[i + 1, j - 1], imageTemp[i + 1, j + 1]);
                    }
                }
            }
            image = imageTemp;
        }

        /// <summary>
        /// Zoom in from the center of the image
        /// </summary>
        /// <param name="ratio">must be more</param>
        public void Zoom(float ratio)
        {
            int height = (int)((float)image.GetLength(0) / ratio);
            int width = (int)((float)image.GetLength(1) / ratio);
            Console.WriteLine("height : " + height + " width : " + width);
            Pixel[,] imageTemp = new Pixel[height, width];
            if (ratio > 1)
            {
                Console.WriteLine("Write image");
                for (int i = 0; i < height; i++)
                    for (int j = 0; j < width; j++)
                        imageTemp[i, j] = image[(image.GetLength(0) - height) / 2 + i, (image.GetLength(1) - width) / 2 + j];
                image = imageTemp;
            }
            else if (ratio < 1)
            {
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if ((height - image.GetLength(0)) / 2 > i || (width - image.GetLength(1)) / 2 > j || (height + image.GetLength(0)) / 2 - 1 < i || (width + image.GetLength(1)) / 2 - 1 < j) imageTemp[i, j] = Pixel.Black;
                        else imageTemp[i, j] = image[i - (height - image.GetLength(0)) / 2, j - (width - image.GetLength(1)) / 2];
                    }
                }
                image = imageTemp;
            }
        }

        /// <summary>
        /// Returns the closest pixel with manhattan distance
        /// </summary>
        /// <param name="image"></param>
        /// <param name="line">line index of the pixel</param>
        /// <param name="column">column index of the pixel</param>
        /// <param name="dist">do not enter numbre (for recursion)</param>
        /// <returns>the closest Pixel of the pixel at index [line,column]</returns>
        private static Pixel ClosePixel(Pixel[,] image, int line,int column, int dist = 1)
        {
            if (dist >= image.GetLength(0) && dist >= image.GetLength(1)) return Pixel.Black;
            int distance = -1;
            Pixel result = new Pixel(0);
            for (int i = line-dist; i < line+dist+1; i++)
            {
                for (int j = column-dist; j < column+dist+1; j++)
                {
                    int manhattanDistance = Math.Abs(i - line) + Math.Abs(j - column);
                    if (i != j && i >= 0 && i < image.GetLength(0) && j >= 0 && j < image.GetLength(1) && image[i, j] != null && (distance == -1 || manhattanDistance < distance)) result.SetValueTo(image[i, j]);
                    if (distance == 1) return result;
                }
            }
            if (distance == -1) return ClosePixel(image, line, column, dist + 1);
            else return result;
        }

        /// <summary>
        /// Rotates the image clockwise with positive angles
        /// </summary>
        /// <param name="teta">the angle of the rotation in degrees</param>
        /// <param name="fix">(not working) crops the image to fit in the frame with no black borders</param>
        public void Rotate(float teta, bool fix = true)
        {
            teta %= 360;
            if (teta > 180) teta -= 360;
            else if (teta < -180) teta += 360;
            if (Math.Abs(teta) > 135)
            {
                Pixel[,] imageTemp = new Pixel[image.GetLength(0), image.GetLength(1)];
                for (int i = 0; i < image.GetLength(0); i++)
                    for (int j = 0; j < image.GetLength(1); j++)
                        imageTemp[i, j] = image[image.GetLength(0) - i - 1, image.GetLength(1) - j - 1];
                image = imageTemp;
                if (teta < 0) teta += 180;
                else teta -= 180;
            }
            if (Math.Abs(teta) > 45)
            {
                Pixel[,] imageTemp = new Pixel[image.GetLength(1), image.GetLength(0)];
                for (int i = 0; i < image.GetLength(0); i++)
                {
                    for (int j = 0; j < image.GetLength(1); j++)
                    {
                        if (teta > 0) imageTemp[j, i] = image[image.GetLength(0) - i - 1, j];
                        else imageTemp[j, i] = image[i, image.GetLength(1) - j - 1];
                    }
                        
                }
                    
                image = imageTemp;
                if (teta < 0) teta += 90;
                else teta -= 90;
            }
            float tetaRad = teta * (float)Math.PI / 180f; //convert degrees to radian
            if (teta != 0) RotateAngle(tetaRad, fix);
        }

        /// <summary>
        /// Rotate an image with any float Values
        /// </summary>
        /// <param name="teta">anfgle of rotation in radians</param>
        /// <param name="fix">(not working) crop the image to fit the frame (no black borders)</param>
        private void RotateAngle(float teta, bool fix)
        {
            Pixel[,] imageTemp = new Pixel[image.GetLength(0), image.GetLength(1)];
            //define the middle of the image
            int x0 = image.GetLength(1) / 2;
            int y0 = image.GetLength(0) / 2;
            // Define the variables for the angle
            float cosTeta = (float)Math.Cos(teta);
            float sinTeta = (float)Math.Sin(teta);
            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    int x = (int)(cosTeta * (j - x0) + sinTeta * (i - y0)) + x0;
                    int y = (int)(-sinTeta * (j - x0) + cosTeta * (i - y0)) + y0;
                    if (x < 0 || y < 0 || x >= image.GetLength(1) || y >= image.GetLength(0)) imageTemp[i, j] = Pixel.Black;
                    else imageTemp[i, j] = new Pixel(image[y, x]);
                }
            }
            for (int i = 0; i < image.GetLength(0); i++)
                for (int j = 0; j < image.GetLength(1); j++)
                    if (imageTemp[i, j] == null) imageTemp[i, j] = Pixel.Black; 
            image = imageTemp;
            if (false) //fix
            {
                /* https://stackoverflow.com/questions/21346670/cropping-rotated-image-with-same-aspect-ratio */
                float ratio = (float)image.GetLength(1) / (float)image.GetLength(0);
                float height = 2f * (float)image.GetLength(0) / (float)Math.PI + 1;
                float width = height * ratio;
                Console.Write(image.GetLength(0) + " " + image.GetLength(1) + " " + height + " " + width);
                CropFromMiddle((int)height, (int)width);
            }
        }

        /// <summary>
        /// Inverse the color of the image
        /// </summary>
        public void Inverse()
        {
            foreach (Pixel pix in image) pix.Inverse();
        }

        /// <summary>
        /// Crops the image from the top-left corner
        /// </summary>
        /// <param name="height">height of the new image</param>
        /// <param name="width">width of the new image</param>
        public void Crop(int height, int width)
        {
            if (height < image.GetLength(0) && width < image.GetLength(1))
            {
                Pixel[,] imageTemp = new Pixel[height, width];
                for (int i = 0; i < height; i++)
                    for (int j = 0; j < width; j++)
                        imageTemp[i, j] = image[i, j];
                image = imageTemp;
            }
        }

        /// <summary>
        /// Crops the image keeping the middle point
        /// </summary>
        /// <param name="height">height of the new image</param>
        /// <param name="width">width of the new image</param>
        public void CropFromMiddle(int height, int width)
        {
            if (height < image.GetLength(0) && width < image.GetLength(1))
            {
                Pixel[,] imageTemp = new Pixel[height, width];
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        imageTemp[i, j] = image[height / 2 + i, width / 2 + j];
                    }
                }
                image = imageTemp;
            }
        }

        /// <summary>
        /// Crop an image with padding from Left Top Right and Left
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        public void CropFromLTRB(int left, int top, int right, int bottom)
        {
            // Check if it can crop with these values
            if(image.GetLength(0) - top - bottom > 0 && image.GetLength(1) - left - right > 0)
            {
                Pixel[,] imageTemp = new Pixel[image.GetLength(0) - top - bottom, image.GetLength(1) - left - right];
                for (int i = 0; i < imageTemp.GetLength(0); i++)
                    for (int j = 0; j < imageTemp.GetLength(1); j++)
                        imageTemp[i, j] = image[i + top, j + left];
                image = imageTemp;
            }
        }

        //----------------------- Convolution matrix -------------------------------
        /// <summary>
        /// Does a convolution with an image
        /// </summary>
        /// <param name="a">the kernel</param>
        public void Convolution(float[,] a)
        {
            Pixel[,] imageTemp = new Pixel[image.GetLength(0), image.GetLength(1)];
            float norm = Normalization(a);
            for (int i = 0; i < imageTemp.GetLength(0); i++)
            {
                for (int j = 0; j < imageTemp.GetLength(1); j++)
                {
                    Pixel[,] subArray = Subarray(image, i, j, (a.GetLength(1) - 1) / 2);
                    imageTemp[i, j] = Convolution(a, subArray, norm);
                }
            }
            image = imageTemp;
        }

        /// <summary>
        /// Creates a square subarray of a 2D array from the middlepoint [a,b] and size neighbours*2+1
        /// </summary>
        /// <param name="image">the array containing the new array</param>
        /// <param name="a">middle point line index</param>
        /// <param name="b">middle point columns index</param>
        /// <param name="neighbours">the number of neighbours from the middle point</param>
        /// <returns>the subarray</returns>
        private static Pixel[,] Subarray(Pixel[,] image, int a, int b, int neighbours)
        {
            Pixel[,] result = new Pixel[2 * neighbours + 1, 2 * neighbours + 1];
            for (int i = a-neighbours; i < a+neighbours+1; i++)
            {
                for (int j = b-neighbours; j < b+neighbours+1; j++)
                {
                    int iGrid = i;
                    int jGrid = j;
                    if (iGrid < 0) iGrid *= -1;
                    if (jGrid < 0) jGrid *= -1;
                    if (iGrid >= image.GetLength(0)) iGrid = 2 * image.GetLength(0) - iGrid - 1;
                    if (jGrid >= image.GetLength(1)) jGrid = 2 * image.GetLength(1) - jGrid - 1;
                    result[i + neighbours - a, j + neighbours - b] = new Pixel(image[iGrid, jGrid]);
                }
            }
            return result;
        }

        /// <summary>
        /// multiply a subarray with the kernel
        /// </summary>
        /// <param name="a">kernel</param>
        /// <param name="b">square 2D subarray of pixels</param>
        /// <param name="norm">normalization of the kernel</param>
        /// <returns>the result</returns>
        private static Pixel Convolution(float[,] a, Pixel[,] b, float norm)
        {
            if(a.GetLength(0)==b.GetLength(0) && a.GetLength(1) == b.GetLength(1))
            {
                Pixel result = Pixel.Black;
                for (int i = 0; i < a.GetLength(0); i++)
                {
                    for (int j = 0; j < a.GetLength(1); j++)
                    {
                        result += (a[i, j] / norm) * b[i, j];
                    }
                }
                return result;
            }
            return Pixel.Black;
        }

        /// <summary>
        /// Computes the normalization of a kernel
        /// </summary>
        /// <param name="a">the kernel</param>
        /// <returns>the norm</returns>
        private static float Normalization(float[,] a)
        {
            float nNegtive = 0;
            float nPositive = 0;
            foreach(float f in a)
            {
                if (f < 0) nNegtive -= f;
                else nPositive += f;
            }
            if (nPositive < nNegtive) nPositive = nNegtive;
            Console.Write(nPositive);
            return nPositive;
        }

        //-----------Some kernels----------------------

        /// <summary>
        /// Gaussian Blur kernel
        /// </summary>
        public static float[,] GaussianBlur5
        {
            get
            {
                float[,] blur = { { 1, 4, 7, 4, 1 }, { 4, 16, 26, 16, 4 }, { 7, 26, 41, 26, 7 }, { 4, 16, 26, 16, 4 }, { 1, 4, 7, 4, 1 } };
                return blur;
            }
            
        }

        /// <summary>
        /// Simple Blur Kernel
        /// </summary>
        public static float[,] Blur
        {
            get
            {
                float[,] blur = new float[3, 3];
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        blur[i, j] = 1;
                 return blur;
            }
            
        }

        /// <summary>
        /// Auto EdgeDetection kernel
        /// </summary>
        public static float[,] EdgeDetection
        {
            get
            {
                float[,] ed = { { -1, -1, -1 }, { -1, 8, -1 }, { -1, -1, -1 } };
                return ed;
            }
        }

        /// <summary>
        /// for horizontal edge detection
        /// </summary>
        public static float[,] HorizontalEdgeDetection
        {
            get
            {
                float[,] result = { { -1, -1, -1 }, { 0, 0, 0 }, { 1, 1, 1 } };
                return result;
            }
        }

        public static float[,] VerticalEdgeDetection
        {
            get
            {
                float[,] result = { { 1, 1, 1 }, { 0, 0, 0 }, { -1, -1, -1 } };
                return result;
            }
        }

        /* Cryptography */

        /// <summary>
        /// Hide the image into another image
        /// </summary>
        /// <param name="img">image to hide our image</param>
        public void Encrypt(MyImage img)
        {
            int i = 0, j = 0;
            while (image.GetLength(0) >= img.image.GetLength(0) || image.GetLength(1) >= img.image.GetLength(1)) img.IncreaseSize();
            img.CropFromMiddle(image.GetLength(0), image.GetLength(1));
            for (i = 0; i < image.GetLength(0); i++)
            {
                for (j = 0; j < image.GetLength(1); j++)
                {
                    byte[] rBits1 = ByteToBit(image[i, j].R);
                    byte[] rBits2 = ByteToBit(img.image[i, j].R);
                    byte[] gBits1 = ByteToBit(image[i, j].G);
                    byte[] gBits2 = ByteToBit(img.image[i, j].G);
                    byte[] bBits1 = ByteToBit(image[i, j].B);
                    byte[] bBits2 = ByteToBit(img.image[i, j].B);
                    Insert(rBits2, SubArray(rBits1, 4, 4), 0);
                    Insert(gBits2, SubArray(gBits1, 4, 4), 0);
                    Insert(bBits2, SubArray(bBits1, 4, 4), 0);
                    image[i, j].R = BitToByte(rBits2);
                    image[i, j].G = BitToByte(gBits2);
                    image[i, j].B = BitToByte(bBits2);
                }
            }
        }

        /// <summary>
        /// Decrypts the image hidden
        /// </summary>
        /// <returns>the image without the encrypted image below</returns>
        public MyImage Decrypt()
        {
            byte[] zero = { 0, 0, 0, 0 };
            Pixel[,] img = new Pixel[image.GetLength(0), image.GetLength(1)];
            MyImage result = new MyImage(img);
            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    byte[] rBits = ByteToBit(image[i, j].R);
                    byte[] gBits = ByteToBit(image[i, j].G);
                    byte[] bBits = ByteToBit(image[i, j].B);
                    byte[] rBits2 = ByteToBit(image[i, j].R);
                    byte[] gBits2 = ByteToBit(image[i, j].G);
                    byte[] bBits2 = ByteToBit(image[i, j].B);
                    //insert creation of new image
                    Insert(rBits, SubArray(rBits, 0, 4), 4);
                    Insert(gBits, SubArray(gBits, 0, 4), 4);
                    Insert(bBits, SubArray(bBits, 0, 4), 4);
                    Clean(rBits, 0, 4);
                    Clean(gBits, 0, 4);
                    Clean(bBits, 0, 4);
                    Clean(rBits2, 0, 4);
                    Clean(gBits2, 0, 4);
                    Clean(bBits2, 0, 4);
                    image[i, j].R = BitToByte(rBits);
                    image[i, j].G = BitToByte(gBits);
                    image[i, j].B = BitToByte(bBits);
                    img[i, j] = new Pixel(BitToByte(rBits2), BitToByte(gBits2), BitToByte(bBits2));
                }
            }
            return result;
        }

        /// <summary>
        /// Convert bits to a byte
        /// </summary>
        /// <param name="bits">array containing bits (1 and 0s)</param>
        /// <returns>byte value corresponding to the bits</returns>
        private static byte BitToByte(byte[] bits)
        {
            byte result = 0;
            for (int i = 0; i < bits.Length; i++)
                result += (byte)(bits[i] * Math.Pow(2, i));
            return result;
        }

        /// <summary>
        /// Convert a byte into array of 0s and 1s
        /// </summary>
        /// <param name="n">byte value to convert</param>
        /// <returns>the array of bits</returns>
        private static byte[] ByteToBit(byte n)
        {
            byte[] result = new byte[8];
            for (int i = 7; i >= 0; i--)
            {
                if (n >= Math.Pow(2, i))
                {
                    result[i] = 1;
                    n -= (byte)Math.Pow(2, i);
                }
                else result[i] = 0;
            }
            return result;
        }

        /// <summary>
        /// Only keeps the yellow color
        /// </summary>
        public void RedFilter()
        {
            foreach (Pixel p in image) p.RedFilter();
        }
    }
}
