using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReedSolomon;

namespace TD2_Projet_Info_Maxence_Raballand
{
    /// <summary>
    /// Encode or Decode a QRCode with an instance of the class MyImage
    /// </summary>
    public class QRCode : MyImage
    {
        private byte[,] code;
        private string encoding;
        private string length;
        private string messageBits;
        private string error;
        string message;
        byte type;

        /// <summary>
        /// Create the QRCode and the image corresponding
        /// </summary>
        /// <param name="message">message to encode in the QRCode</param>
        private QRCode(string message)
        {
            offsetSize = 54;
            imageType = "BM";
            this.message = message.ToUpper();
            encoding = "0010"; //Alphanumeric
            length = Int_To_Bits(message.Length, 9);
            messageBits = Encode_Message();
            int eccCount = 0;
            if (type == 1) eccCount = 7;
            else eccCount = 10;
            byte[] error = ReedSolomonAlgorithm.Encode(BitsToByte(encoding + length + messageBits), eccCount, ErrorCorrectionCodeType.QRCode);
            this.error = ByteToBit(error);
            Console.WriteLine(encoding.Length + length.Length + messageBits.Length + this.error.Length);
            Console.WriteLine(encoding + length + messageBits + this.error);
            DrawCanvas();
            WriteData();
            DrawErrorCorrection();
            int size = 6;
            image = new Pixel[size * (code.GetLength(0) + 2), size * (code.GetLength(1) + 2)];
            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    if (i < size || j < size || i >= code.GetLength(0) * size + size || j >= code.GetLength(1) * size + size) image[i, j] = Pixel.White;
                    else if (code[i / size - 1, j / size - 1] == 0) image[i, j] = Pixel.White;
                    else image[i, j] = Pixel.Black;
                }
            }
        }

        /// <summary>
        /// Decode a QRCode
        /// </summary>
        /// <param name="img">image containing the QRCode</param>
        private QRCode(MyImage img)
        {
            // Copy image on this object
            image = new Pixel[img.image.GetLength(0), img.image.GetLength(1)];
            for (int i = 0; i < image.GetLength(0); i++)
                for (int j = 0; j < image.GetLength(1); j++)
                    image[i, j] = new Pixel(img.image[i, j]);
            /* https://www.researchgate.net/publication/221337868_Fast_QR_Code_Detection_in_Arbitrarily_Acquired_Images */
            //Assume image is flat on the surface
            //Find the size of a finder and the size of a code to determine the version 
            //and the size of a bit to be accurate while reading
            // plus save the start index of QRCode;
            // Different size horizontal and vertical (if image is warped
            int sizeOfFinderHorizontal = 0;
            int sizeOfCodeHorizontal = 0;
            int sizeOfFinderVertical = 0;
            int sizeOfCodeVertical = 0;
            bool b = true;
            int line = 0;
            int column = 0;
            int startLine = 0;
            int startColumn = 0;
            //Searching
            while (b)
            {
                column = 0;
                while (column < image.GetLength(1) && !image[line, column].isCloseToBlack) column++;
                startColumn = column;
                startLine = line;
                // For horizontal size
                while(column < image.GetLength(1) && image[startLine, column].isCloseToBlack)
                {
                    b = false;
                    sizeOfFinderHorizontal++;
                    column++;
                }
                // For Vertical size
                while(!b && line < image.GetLength(0) && image[line, startColumn].isCloseToBlack)
                {
                    sizeOfFinderVertical++;
                    line++;
                }
                if (!b)
                {
                    // Horizontal size
                    for (int j = image.GetLength(1) - 1; j >= 0 && sizeOfCodeHorizontal==0; j--)
                        if (image[startLine, j].isCloseToBlack) sizeOfCodeHorizontal = j - startColumn;
                    // Vertical Size
                    for (int j = image.GetLength(0) - 1; j >= 0 && sizeOfCodeVertical == 0; j--)
                        if (image[j, startColumn].isCloseToBlack) sizeOfCodeVertical = j - startLine;
                }
                line++;
            }
            //Compute the size of a bit (finder is 7 pixels wide)
            int bitSizeHorizontal = sizeOfFinderHorizontal / 7;
            int bitSizeVertical = sizeOfFinderVertical / 7;
            int approxSize = sizeOfCodeHorizontal / bitSizeHorizontal;
            //Determine the version of QR based on the size
            if (approxSize < 23) type = 1;
            else type = 2;
            //Now read the QRCode but first Draw the Canvas
            DrawCanvas();
            //Read DAta
            string messageBits = ReadData(bitSizeHorizontal, bitSizeVertical, startLine, startColumn);
            // Get rid of the last seven bits if type 2
            if (type == 2) messageBits = messageBits.Substring(0, 352);
            Console.WriteLine(messageBits);
            // Get The Error count
            int eccCount = 7 * 8;
            if (type == 2) eccCount = 10 * 8;
            //Split error bits and message bits
            string errorBits = messageBits.Substring(messageBits.Length - eccCount, eccCount);
            messageBits = messageBits.Substring(0, messageBits.Length - eccCount);
            // Decode with Reed Solomon Algorithm
            byte[] messageBytes = BitsToByte(messageBits);
            byte[] errorBytes = BitsToByte(errorBits);
            byte[] finalMessage = ReedSolomonAlgorithm.Decode(messageBytes, errorBytes, ErrorCorrectionCodeType.QRCode);
            messageBits = ByteToBit(finalMessage);
            // Assume the image is alphanumeric and get rid of the length
            messageBits = messageBits.Substring(4, messageBits.Length - 4);
            Decode_Message(messageBits);
        }

        /// <summary>
        /// Encodes a message into a QRCode
        /// </summary>
        /// <param name="message">message to encode</param>
        /// <returns>the instance of class QRCode corresponding to the message</returns>
        public static QRCode Encode(string message)
        {
            return new QRCode(message);
        }

        /// <summary>
        /// Decode a QRCode
        /// </summary>
        /// <param name="img">the image containing the QRCode</param>
        /// <returns>the message contatained in the QRCode</returns>
        public static string Decode(MyImage img)
        {
            QRCode init = new QRCode(img);
            return init.message;
        }

        /// <summary>
        /// convert a string containing 0s and 1s to a byte array
        /// </summary>
        /// <param name="bits">string containing bits</param>
        /// <returns>the array of byte corresponding to the bits</returns>
        private byte[] BitsToByte(string bits)
        {
            if (bits.Length % 8 == 0)
            {
                byte[] result = new byte[bits.Length / 8];
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = 0;
                    for (int j = 0; j < 8; j++)
                    {
                        if(bits[i * 8 + j]=='1')
                            result[i] += (byte)(Math.Pow(2, 7 - (double)j));
                    }
                }
                return result;
            }
            else return null;
        }

        /// <summary>
        /// Choose the version of QRCode needed for the message length
        /// </summary>
        private void ChooseVersion()
        {
            if (message.Length < 26) type = 1;
            else if (message.Length < 48) type = 2;
        }

        /// <summary>
        /// convert an integers into a string containing bits with specified size
        /// </summary>
        /// <param name="n">integer to convert</param>
        /// <param name="size">size of the string</param>
        /// <returns>the string of bits corresponding to the integer given</returns>
        public static string Int_To_Bits(int n, int size)
        {
            string result = "";
            int pow = 0;
            while (--size >= 0 && n != 0)
            {
                if ((pow = (int)Math.Pow(2, size)) <= n)
                {
                    result += '1';
                    n -= pow;
                }
                else result += '0';
            }
            for (int i = size; i >= 0; i--) result += '0';
            return result;
        }

        /// <summary>
        /// Convert a string of bits into an integer
        /// </summary>
        /// <param name="bits">string of bits</param>
        /// <returns>integer corresponding to the string of bits</returns>
        public static int Bits_To_Int(string bits)
        {
            int result = 0;
            for (int i = 0; i < bits.Length; i++)
                if (bits[i] == '1') result += (int)Math.Pow(2, bits.Length - i - 1);
            return result;
        }

        /// <summary>
        /// Encode the message in alphanumeric with the error
        /// </summary>
        /// <returns>string of bits corresponding to the encoded message</returns>
        public string Encode_Message()
        {
            string result = "";
            char[] alphas = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ $%*+-./:".ToCharArray();
            for (int i = 0; i < message.Length; i+=2)
            {
                string pair = "";
                pair += message[i];
                if (i < message.Length - 1) pair += message[i + 1];
                if (pair.Length == 2) result += Int_To_Bits(45 * Array.IndexOf(alphas, pair[0]) + Array.IndexOf(alphas, pair[1]), 11);
                else result += Int_To_Bits(Array.IndexOf(alphas, pair[0]), 6);
            }
            //choose version and determine the number of bits to fill
            ChooseVersion();
            int maxBits = 0;
            if (type == 1) maxBits = 152;
            else if (type == 2) maxBits = 272;
            // terminator
            for (int i = 0; i < 4 && maxBits > result.Length + 9 + 4; i++)
                result += '0';
            //multiple of 8
            while ((9 + 4 + result.Length) % 8 != 0 && maxBits > result.Length + 9 + 4) result += '0';
            //pad bytes to fill the rest
            int loop = 0;
            while (maxBits > 9 + 4 + result.Length)
            {
                if (loop % 2 == 0) result += "11101100";
                else result += "00010001";
                loop++;
            }
            return result;
        }

        /// <summary>
        /// Encode the error into a string of bits
        /// </summary>
        /// <returns>the string of bits corresponding to the error</returns>
        public string EncodeError()
        {
            string result = "";
            int eccCount = 0;
            if (type == 1) eccCount = 7;
            else if (type == 2) eccCount = 10;
            byte[] code = ReedSolomonAlgorithm.Encode(Encoding.UTF8.GetBytes(message), eccCount);
            result += ByteToBit(code);
            return result;
        }

        /// <summary>
        /// Convert an array of byte into a string of bits
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns>the string of bits corresponding to the bytes</returns>
        public static string ByteToBit(byte[] bytes)
        {
            string result = "";
            foreach (byte b in bytes)
                result += Int_To_Bits((int)b, 8);
            return result;
        }

        /// <summary>
        /// Draw the canvas of the image
        /// </summary>
        /// <param name="createImage">if true create an image of pixels</param>
        private void DrawCanvas()
        {
            // 0 : white 1 : black 2 : reserved 3 : non - written
            code = new byte[type * 4 + 17, type * 4 + 17];
            for (int i = 0; i < code.GetLength(0); i++)
                for (int j = 0; j < code.GetLength(1); j++)
                    code[i, j] = 3; //bit inutilisé
            //Finders
            AddFinder(0, 0);
            AddFinder(type * 4 + 10, 0);
            AddFinder(0, type * 4 + 10);
            //Alignement
            if (type == 2)
            {
                for (int i = 16; i < 21; i++)
                {
                    for (int j = 16; j < 21; j++)
                    {
                        if (i == 16 || j == 16 || i == 20 || j == 20 || (i==18 && j==18)) code[i, j] = 1;
                        else code[i, j] = 0;
                    }
                }
            }     
            //Timing patterns
            int k = 8;
            while (code[k, 6] != 1)
            {
                code[k, 6] = 1;
                code[k+1, 6] = 0;
                code[6, k] = 1;
                code[6, k+1] = 0;
                k += 2;
            }
            //DarkModule
            code[(4 * type) + 9, 8] = 1;
            //Reserve format information area
                //Top Left
            for (int i = 0; i < 9; i++)
            {
                if (code[i, 8] == 3)
                {
                    code[i, 8] = 2;
                    code[8, i] = 2;
                }
            }
                //Bottom left and top right
            for (int i = code.GetLength(1)-1; i >= code.GetLength(1) - 8; i--)
            {
                if (code[i, 8] == 3) code[i, 8] = 2;
                code[8, i] = 2;
            }
        }

        /// <summary>
        /// Write the Date onto the canvas
        /// </summary>
        private void WriteData()
        {
            string message = encoding + length + messageBits + error;
            if (type == 2) message += "0000000";
            int position = 0; //position of the bit
            int i = code.GetLength(0) - 1; // line
            int j = code.GetLength(1) - 1; // column
            bool upward = true; //direction
            while (true)
            {
                while (true)
                {
                    if (code[i, j] == 3)
                    {
                        code[i, j] = toByte(message, position);
                        position++;
                        //mask 0
                        if (code[i, j] == 1) code[i, j] = 1;
                        else if (code[i, j] == 0) code[i, j] = 0;
                        //mask 0
                        if ((i + j) % 2 == 0) code[i, j] = (byte)(1 - code[i, j]);
                    }
                    if (code[i, j - 1] == 3)
                    {
                        code[i, j - 1] = toByte(message, position);
                        position++;
                        //mask 0
                        if (code[i, j - 1] == 1) code[i, j - 1] = 1;
                        else if (code[i, j - 1] == 0) code[i, j - 1] = 0;
                        //mask 0
                        if ((i + j - 1) % 2 == 0) code[i, j - 1] = (byte)(1 - code[i, j - 1]);
                    }
                    if ((i == 0 && upward) || (i == code.GetLength(0) - 1 && !upward)) break;
                    else if (upward) i--;
                    else i++;
                }
                if (j == 1) break;
                upward = !upward;
                if (j == 8) j -= 3;
                else j -= 2;
            }
        }

        /// <summary>
        /// Convert a char in the message into a bit
        /// </summary>
        /// <param name="message">the message</param>
        /// <param name="position">the position of the character</param>
        /// <returns></returns>
        private byte toByte(string message,int position)
        {
            if (message[position] == '1') return 1;
            else return 0;
        }

        /// <summary>
        /// Adds the finder patterns
        /// </summary>
        /// <param name="x">line index of the finder pattern</param>
        /// <param name="y">column indes of the finder pattern</param>
        private void AddFinder(int x, int y)
        {
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (i == 0 || j == 0 || i == 6 || j == 6 || (i > 1 && i < 5 && j > 1 && j < 5)) code[x + i, y + j] = 1;
                    else code[x + i, y + j] = 0;
                }
            }
            //Separators
            for (int i = -1; i < 8; i++)
            {
                for (int j = -1; j < 8; j++)
                {
                    if (x + i >= 0 && y + j >= 0 && x + i < code.GetLength(0) && y + j < code.GetLength(1) && code[i + x, j + y] == 3) code[i + x, j + y] = 0;
                }
            }
        }

        /// <summary>
        /// Draw the 14 bits of mask and version
        /// </summary>
        private void DrawErrorCorrection()
        {
            string result = "111011111000100";

            //Write in the QrCode
            int k = code.GetLength(0) - 1;
            int position = 0;
            while (k >= 0)
            {
                if (code[k, 8] == 2)
                {
                    code[k, 8] = toByte(result, position);
                    //if ((k + 8) % 2 == 0) code[k, 8] = (byte)(1 - code[k, 8]);
                    position++;
                }
                k--;
            }
            k = 0;
            position = 0;
            while (k < code.GetLength(0))
            {
                if (code[8, k] == 2)
                {
                    code[8, k] = toByte(result, position);
                    //if ((k + 8) % 2 == 0) code[8, k] = (byte)(1 - code[8, k]);
                    position++;
                }
                k++;
            }

        }

        //------------------------------functions for Decoding------------------------------------------

        /// <summary>
        /// Read the date inside a QRCode
        /// </summary>
        /// <param name="bitSizeHorizontal">size of a bit in pixel horizontally</param>
        /// <param name="bitSizeVertical">size of a bit in pixel vertically</param>
        /// <param name="startLine">top-left line index of the QRCode</param>
        /// <param name="startColumn">top-left column index of the QRCode</param>
        /// <returns>the string corresponding to the data read</returns>
        public string ReadData(int bitSizeHorizontal, int bitSizeVertical, int startLine, int startColumn)
        {
            //an index is [line*bitSizeVertical+startLine+bitSizeVertical/2,column*bitSizeHorizontal+startColumn+bitSizeHorizontal/2]
            //assume it's 0 masked
            int i = code.GetLength(0) - 1;
            int j = code.GetLength(1) - 1;
            string message = "";
            bool upward = true;
            while (true)
            {
                while (true)
                {
                    if (code[i, j] == 3)
                    {
                        int temp = 0;
                        if (image[i * bitSizeVertical + startLine + bitSizeVertical / 2, j * bitSizeHorizontal + startColumn + bitSizeHorizontal / 2].isCloseToBlack) temp = 1; 
                        if ((i + j) % 2 == 0) temp = 1 - temp; //mask 0 conditions
                        message += temp;
                    }
                    if (code[i, j - 1] == 3)
                    {
                        int temp = 0;
                        if (image[i * bitSizeVertical + startLine + bitSizeVertical / 2, (j - 1) * bitSizeHorizontal + startColumn + bitSizeHorizontal / 2].isCloseToBlack) temp = 1;
                        if ((i + (j - 1)) % 2 == 0) temp = 1 - temp; //mask 0 conditions
                        message += temp;
                    }
                    if ((i == 0 && upward) || (i == code.GetLength(0) - 1 && !upward)) break;
                    else if (upward) i--;
                    else i++;
                }
                if (j == 1) break;
                upward = !upward;
                if (j == 8) j -= 3;
                else j -= 2;
            }
            return message;
        }

        /// <summary>
        /// Decode the message inside the QRCode
        /// </summary>
        /// <param name="messageBits">the message in bits</param>
        private void Decode_Message(string messageBits)
        {
            char[] alphas = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ $%*+-./:".ToCharArray();
            message = "";
            // 9 first bits are the length of the message
            int length = Bits_To_Int(messageBits.Substring(0, 9));
            // get rid of the size info
            messageBits = messageBits.Substring(9, messageBits.Length - 9);
            //Length of message in bits
            int bitsLength = 11 * (length - length % 2) / 2 + 6 * (length % 2);
            for (int i = 0; i < bitsLength; i+=11)
            {
                if(i+11 <= bitsLength)
                {
                    //Code if it's a pair
                    int temp = Bits_To_Int(messageBits.Substring(i, 11));
                    int temp2 = temp % 45;
                    temp = (temp - temp2) / 45;
                    message += alphas[temp];
                    message += alphas[temp2];
                }
                else
                {
                    //code if it's a only letter
                    int temp = Bits_To_Int(messageBits.Substring(i, 6));
                    message += alphas[temp];
                }
            }
        }

        private int[,] FindFinderPatterns()
        {
            List<Tuple<int, int>> coord = new List<Tuple<int, int>>();
            //Search for the pattern w:b:w:bbb:w:b:w w for white and b for black as refered in https://www.researchgate.net/publication/221337868_Fast_QR_Code_Detection_in_Arbitrarily_Acquired_Images
            int[] pattern = { 0, 1, 0, 1, 1, 1, 0, 1, 0 }; // 0 for white and 1 for black and 2 for other colors....
            for (int i = 0; i < image.GetLength(0); i++)
            {
                int[] line = new int[image.GetLength(1)];
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    if (image[i, j].isCloseToWhite) line[j] = 0;
                    else if (image[i, j].isCloseToBlack) line[j] = 1;
                    else line[j] = 2;
                }
            }
            return null;
        }

        /// <summary>
        /// Search for the pattern (not finished) if the image is tilted
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="array"></param>
        /// <returns>index of the pattern</returns>
        public static int[] MatchingPattern(int[] pattern, int[] array)
        {
            List<int> coord = new List<int>();
            int[] patternTemp = new int[1];
            int i = 0;
            do
            {
                i++;
                patternTemp = new int[i * pattern.Length];
                for (int j = 0; j < patternTemp.Length; j++)
                    patternTemp[j] = pattern[j / i];
                for (int j = 0; j < array.Length - patternTemp.Length; j++)
                    if (patternTemp.SequenceEqual(SubArray(array, j, patternTemp.Length))) coord.Add(j + patternTemp.Length / 2);
            } while (patternTemp.Length < array.Length);
            return coord.ToArray();
        }
    }
}
