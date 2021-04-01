using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TD2_Projet_Info_Maxence_Raballand;

namespace ProjetInfoUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestSubArray()
        {
            byte[] array = { 12, 23, 23, 45, 12, 65, 43, 13 };
            byte[] result = { 23, 45, 12 };
            byte[] test = MyImage.SubArray(array, 2, 3);
            CollectionAssert.AreEqual(result, test);
        }

        [TestMethod]
        public void TestInt_To_Bits()
        {
            string result = "000001011";
            Assert.AreEqual(result, QRCode.Int_To_Bits(11, 9));
        }

        [TestMethod]
        public void TestEncodeMessage()
        {
            QRCode qr = QRCode.Encode("HELLO WORLD");
            string result = "0110000101101111000110100010111001011011100010011010100001101000000111011000001000111101100000100011110110000010001111011000001000111101100";
            Assert.AreEqual(result, qr.Encode_Message());
        }

        [TestMethod]
        public void Test()
        {
            char[] alphas = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ $%*+-./:".ToCharArray();
            Assert.AreEqual("01100001011", QRCode.Int_To_Bits(45 * Array.IndexOf(alphas, 'H') + Array.IndexOf(alphas, 'E'), 11));
        }

        [TestMethod]
        public void Bits_To_Int()
        {
            string test = "0100";
            Assert.AreEqual(4, QRCode.Bits_To_Int(test));
        }
    }
}
