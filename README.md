# cs simple image software

&copy; Maxence Raballand 2021

Created for a school project with no image librarys (even bitmap). It can create QR codes, decode them, encode and decode images, and do tons of modifications on bitmap images only. (Jan-April 2020).

## Warning

This was created for a school project using no library so it's not very effecient. You can use the code and link the repo if you want.

## Algorithms

- The Reed Solomon algorithm can be found under [this folder](https://github.com/maxencerb/cs-simple-image-software/tree/master/TD2_Projet_Info_Maxence_Raballand). It was not coded by me. It is used to encode and decode the message (byte <=> alphanumeric) for QRCode.


- QRCode encoder and Decoder using the ReedSolomon Algorithm. I let you read the source code. For simple usage :

```cs
// Create a short message with only alphanumeric characters
// Links work
MyImage img = QRCode.Encode("maxenceraballand.com");
img.Save("path/to/file.bmp");

// To Decode A QRCode
MyImage qrCode = new MyImage("path/to/file.bmp");
string message = QRCode.Decode(qrCode);
Console.WriteLine($"The message is : {message}");
```

- Get color histogram :

```cs
MyImage img = new MyImage("path/to/file.bmp");
string filename = img.Histogram("path/to/histogram.bmp");
Process.Start(filename);
```

- There are many image transform in the [MyImage.cs](https://github.com/maxencerb/cs-simple-image-software/blob/master/TD2_Projet_Info_Maxence_Raballand/MyImage.cs) file. I'll let you discover them.

- Apply convolution kernel to Image

```cs
MyImage img = new MyImage("path/to/file.bmp");

float[,] custom_kernel = { { 1, 4, 7, 4, 1 }, 
                  { 4, 16, 26, 16, 4 }, 
                  { 7, 26, 41, 26, 7 }, 
                  { 4, 16, 26, 16, 4 }, 
                  { 1, 4, 7, 4, 1 } };

// This variable is also in MyImage.GaussianBlur5
// You can find all premade kernels in the MyImage.cs file

float[,] blur = MyImage.GaussianBlur5;
img.Convolution(blur);
string filename = img.Save("path/to/blurred");
Process.Start(filename);
```

- When you encrypt two images, the result will be an image containing all the informations of both images. However, you won't see anything. Decrypt and Encrypt Images :

```cs
MyImage img1 = new MyImage("path/to/img1.bmp");
MyImage img2 = new MyImage("path/to/img2.bmp");

// Encrypt

img1.Encrypt(img2);
string file = img1.Save("path/to/encrypt.bmp");
Process.Start(file);

// Decrypt

img1 = new MyImage("path/to/encrypt.bmp");
img2 = img1.Decrypt();
string file1 = img1.Save("path/to/decrypt1.bmp");
string file2 = img2.Save("path/to/decrypt2.bmp");
Process.Start(file1);
Process.Start(file2);
```

- You can create a fractal image with the functions in the [Fractal.cs](https://github.com/maxencerb/cs-simple-image-software/blob/master/TD2_Projet_Info_Maxence_Raballand/Fractal.cs) file.

## Graphic interface

You can test the graphic interface I created to apply all these algorithm simply. The executable file can be found in [this release](https://github.com/maxencerb/cs-simple-image-software/releases/tag/v1.0).
